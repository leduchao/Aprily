using Aprily.Backend.Common.Constants;
using Aprily.Backend.Common.Results;
using Aprily.Backend.Database;
using Aprily.Backend.Entities;
using Aprily.Backend.Features.Chat.Hubs;
using Aprily.Backend.Features.Chat.Models;
using Aprily.Backend.Features.Users.Services;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

namespace Aprily.Backend.Features.Chat.UseCases.SendDirectMessage;

public record SendDirectMessageResponse(Guid ConversationId, ChatMessageResponse Message);

public sealed class SendDirectMessageCommand(
    Guid conversationId,
    string? content,
    IReadOnlyList<IFormFile>? images = null,
    Guid? replyToMessageId = null)
    : IRequest<Result<SendDirectMessageResponse>>
{
    public Guid ConversationId { get; init; } = conversationId;
    public string? Content { get; init; } = content;
    public IReadOnlyList<IFormFile> Images { get; init; } = images ?? [];
    public Guid? ReplyToMessageId { get; init; } = replyToMessageId;

    public sealed class Handler(
        AppDbContext dbContext,
        ICurrentUser currentUser,
        IHubContext<ChatHub> chatHub,
        IWebHostEnvironment environment)
        : IRequestHandler<SendDirectMessageCommand, Result<SendDirectMessageResponse>>
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ICurrentUser _currentUser = currentUser;
        private readonly IHubContext<ChatHub> _chatHub = chatHub;
        private readonly IWebHostEnvironment _environment = environment;

        public async Task<Result<SendDirectMessageResponse>> Handle(
            SendDirectMessageCommand request,
            CancellationToken cancellationToken)
        {
            var content = string.IsNullOrWhiteSpace(request.Content)
                ? null
                : request.Content.Trim();

            var membership = await _dbContext.ConversationMembers
                .Include(cm => cm.User)
                .Include(cm => cm.Conversation)
                .FirstOrDefaultAsync(cm => cm.Conversation.EntityId == request.ConversationId &&
                    cm.User.EntityId == _currentUser.UserEntityId && !cm.IsDeleted &&
                    !cm.User.IsDeleted && !cm.Conversation.IsDeleted, cancellationToken);

            if (membership is null)
            {
                return Result<SendDirectMessageResponse>.Failure(
                    new Error("chat.conversation_not_found", "Conversation not found"));
            }
            var sender = membership.User;
            var conversation = membership.Conversation;
            var conversationMembers = await _dbContext.ConversationMembers
                .Include(cm => cm.User)
                .Where(cm => cm.ConversationId == conversation.Id && !cm.IsDeleted && !cm.User.IsDeleted)
                .ToListAsync(cancellationToken);

            Message? replyToMessage = null;
            if (request.ReplyToMessageId is not null)
            {
                replyToMessage = await _dbContext.Messages
                    .Include(candidate => candidate.SenderUser)
                    .Include(candidate => candidate.MessageAttachments.Where(attachment => !attachment.IsDeleted))
                    .FirstOrDefaultAsync(
                        candidate =>
                            candidate.EntityId == request.ReplyToMessageId &&
                            candidate.ConversationId == conversation.Id &&
                            !candidate.IsDeleted,
                        cancellationToken);

                if (replyToMessage is null)
                {
                    return Result<SendDirectMessageResponse>.Failure(
                        new Error("chat.reply_message_not_found", "The message being replied to was not found"));
                }
            }

            var now = DateTime.UtcNow;
            var message = new Message
            {
                EntityId = Guid.NewGuid(),
                ConversationId = conversation.Id,
                SenderUserId = sender.Id,
                ReplyToMessageId = replyToMessage?.Id,
                Content = content,
                SentAt = now,
                CreatedAt = now,
                UpdatedAt = now,
                IsDeleted = false
            };

            var storedFilePaths = new List<string>();

            try
            {
                await AddImageAttachments(message, request.Images, storedFilePaths, now, cancellationToken);

                await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

                await _dbContext.Messages.AddAsync(message, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                conversation.LastMessageId = message.Id;
                conversation.LastMessageAt = message.SentAt;

                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                DeleteStoredFiles(storedFilePaths);
                throw;
            }

            var attachments = message.MessageAttachments
                .OrderBy(attachment => attachment.SortOrder)
                .Select(ToResponse)
                .ToList();
            var replyTo = replyToMessage is null
                ? null
                : new ChatMessageReplyResponse(
                    replyToMessage.EntityId,
                    replyToMessage.SenderUser.EntityId,
                    replyToMessage.SenderUser.Username,
                    replyToMessage.Content,
                    replyToMessage.MessageAttachments.Count > 0);

            var responseMessage = new ChatMessageResponse(
                message.EntityId,
                conversation.EntityId,
                sender.EntityId,
                sender.FullName ?? sender.Username,
                sender.AvatarUrl,
                message.Content,
                attachments,
                replyTo,
                [],
                message.SentAt,
                true);

            foreach (var member in conversationMembers)
                await _chatHub.Clients.Group(ChatHub.UserGroup(member.User.EntityId))
                    .SendAsync("messageReceived", responseMessage with { IsMine = member.UserId == sender.Id }, cancellationToken);

            await _chatHub.Clients
                .Groups(conversationMembers.Select(member => ChatHub.UserGroup(member.User.EntityId)).ToArray())
                .SendAsync("conversationUpdated", conversation.EntityId, cancellationToken);

            return Result<SendDirectMessageResponse>.Success(
                new SendDirectMessageResponse(conversation.EntityId, responseMessage));
        }

        private async Task AddImageAttachments(
            Message message,
            IReadOnlyList<IFormFile> images,
            ICollection<string> storedFilePaths,
            DateTime now,
            CancellationToken cancellationToken)
        {
            if (images.Count == 0)
            {
                return;
            }

            var imagesDirectory = Path.Combine(
                _environment.ContentRootPath,
                UploadPaths.RootDirectoryName,
                UploadPaths.ChatImagesDirectoryName);
            Directory.CreateDirectory(imagesDirectory);

            for (var index = 0; index < images.Count; index++)
            {
                var image = images[index];
                var extension = ChatImageUploadRules.GetExtension(image.ContentType);
                var fileName = $"{message.EntityId:N}-{index}-{Guid.NewGuid():N}{extension}";
                var destinationPath = Path.Combine(imagesDirectory, fileName);

                await using (var stream = new FileStream(
                    destinationPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None,
                    81920,
                    FileOptions.Asynchronous))
                {
                    await image.CopyToAsync(stream, cancellationToken);
                }

                storedFilePaths.Add(destinationPath);

                message.MessageAttachments.Add(new MessageAttachment
                {
                    EntityId = Guid.NewGuid(),
                    Type = "image",
                    Url = UploadPaths.GetChatImageUrl(fileName),
                    OriginalFileName = TruncateFileName(Path.GetFileName(image.FileName)),
                    ContentType = image.ContentType,
                    SizeBytes = image.Length,
                    SortOrder = (short)index,
                    CreatedAt = now,
                    UpdatedAt = now,
                    IsDeleted = false
                });
            }
        }

        private static ChatMessageAttachmentResponse ToResponse(MessageAttachment attachment) =>
            new(
                attachment.EntityId,
                attachment.Type,
                attachment.Url,
                attachment.OriginalFileName,
                attachment.ContentType,
                attachment.SizeBytes,
                attachment.Width,
                attachment.Height,
                attachment.SortOrder);

        private static string? TruncateFileName(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }

            return fileName.Length <= 255 ? fileName : fileName[..255];
        }

        private static void DeleteStoredFiles(IEnumerable<string> filePaths)
        {
            foreach (var filePath in filePaths)
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

    }
}
