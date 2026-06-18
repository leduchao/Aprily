using Aprily.Backend.Common.Results;
using Aprily.Backend.Database;
using Aprily.Backend.Database.Connection;
using Aprily.Backend.Entities;
using Aprily.Backend.Features.Chat.Models;
using Aprily.Backend.Features.Users.Services;

using Dapper;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace Aprily.Backend.Features.Chat.UseCases.SendDirectMessage;

public record SendDirectMessageResponse(Guid ConversationId, ChatMessageResponse Message);

public sealed class SendDirectMessageCommand(Guid recipientUserId, string content)
    : IRequest<Result<SendDirectMessageResponse>>
{
    public Guid RecipientUserId { get; init; } = recipientUserId;
    public string Content { get; init; } = content;

    public sealed class Handler(AppDbContext dbContext, ICurrentUser currentUser, IDbConnectionFactory dbConnectionFactory)
        : IRequestHandler<SendDirectMessageCommand, Result<SendDirectMessageResponse>>
    {
        private const string DirectConversationType = "direct";
        private const string UniqueViolation = "23505";

        private readonly AppDbContext _dbContext = dbContext;
        private readonly ICurrentUser _currentUser = currentUser;
        private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;

        public async Task<Result<SendDirectMessageResponse>> Handle(
            SendDirectMessageCommand request,
            CancellationToken cancellationToken)
        {
            var content = request.Content.Trim();

            var sender = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.EntityId == _currentUser.UserEntityId && !u.IsDeleted, cancellationToken);

            if (sender is null)
            {
                return Result<SendDirectMessageResponse>.Failure(
                    new Error("users.user_notFound", "User not found"));
            }

            var recipient = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.EntityId == request.RecipientUserId && !u.IsDeleted, cancellationToken);

            if (recipient is null)
            {
                return Result<SendDirectMessageResponse>.Failure(
                    new Error("chat.recipient_not_found", "Recipient user not found"));
            }

            if (sender.Id == recipient.Id)
            {
                return Result<SendDirectMessageResponse>.Failure(
                    new Error("chat.cannot_message_self", "Cannot send a direct message to yourself"));
            }

            var (userLowId, userHighId) = sender.Id < recipient.Id
                ? (sender.Id, recipient.Id)
                : (recipient.Id, sender.Id);

            if (!await AreFriends(userLowId, userHighId, cancellationToken))
            {
                return Result<SendDirectMessageResponse>.Failure(
                    new Error("chat.not_friends", "You can only send direct messages to friends"));
            }

            var conversation = await GetOrCreateDirectConversation(
                userLowId,
                userHighId,
                sender.Id,
                recipient.Id,
                cancellationToken);

            if (conversation is null)
            {
                return Result<SendDirectMessageResponse>.Failure(
                    new Error("chat.send_failed", "Failed to create or load the direct conversation"));
            }

            var now = DateTime.UtcNow;
            var message = new Message
            {
                EntityId = Guid.NewGuid(),
                ConversationId = conversation.Id,
                SenderUserId = sender.Id,
                Content = content,
                SentAt = now,
                CreatedAt = now,
                UpdatedAt = now,
                IsDeleted = false
            };

            await _dbContext.Messages.AddAsync(message, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            conversation.LastMessageId = message.Id;
            conversation.LastMessageAt = message.SentAt;

            await _dbContext.SaveChangesAsync(cancellationToken);

            var responseMessage = new ChatMessageResponse(
                message.EntityId,
                conversation.EntityId,
                sender.EntityId,
                sender.Username,
                sender.AvatarUrl,
                message.Content,
                message.SentAt,
                true);

            return Result<SendDirectMessageResponse>.Success(
                new SendDirectMessageResponse(conversation.EntityId, responseMessage));
        }

        private async Task<Conversation?> GetOrCreateDirectConversation(
            int userLowId,
            int userHighId,
            int senderId,
            int recipientId,
            CancellationToken cancellationToken)
        {
            var existingConversation = await LoadDirectConversation(userLowId, userHighId, cancellationToken);
            if (existingConversation is not null)
            {
                return existingConversation;
            }

            var now = DateTime.UtcNow;
            var conversation = new Conversation
            {
                EntityId = Guid.NewGuid(),
                Type = DirectConversationType,
                CreatedAt = now,
                UpdatedAt = now,
                IsDeleted = false
            };

            var directConversation = new DirectConversation
            {
                EntityId = Guid.NewGuid(),
                Conversation = conversation,
                UserLowId = userLowId,
                UserHighId = userHighId,
                CreatedAt = now,
                UpdatedAt = now,
                IsDeleted = false
            };

            var senderMember = new ConversationMember
            {
                EntityId = Guid.NewGuid(),
                Conversation = conversation,
                UserId = senderId,
                CreatedAt = now,
                UpdatedAt = now,
                IsDeleted = false
            };

            var recipientMember = new ConversationMember
            {
                EntityId = Guid.NewGuid(),
                Conversation = conversation,
                UserId = recipientId,
                CreatedAt = now,
                UpdatedAt = now,
                IsDeleted = false
            };

            await _dbContext.Conversations.AddAsync(conversation, cancellationToken);
            await _dbContext.DirectConversations.AddAsync(directConversation, cancellationToken);
            await _dbContext.ConversationMembers.AddRangeAsync([senderMember, recipientMember], cancellationToken);

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
                return conversation;
            }
            catch (DbUpdateException exception) when (IsUniqueViolation(exception))
            {
                _dbContext.ChangeTracker.Clear();
                return await LoadDirectConversation(userLowId, userHighId, cancellationToken);
            }
        }

        private async Task<bool> AreFriends(int userLowId, int userHighId, CancellationToken cancellationToken)
        {
            using var conn = await _dbConnectionFactory.CreateConnection();

            return await conn.ExecuteScalarAsync<bool>(
                new CommandDefinition(
                    """
                    SELECT EXISTS (
                        SELECT 1
                        FROM friendships
                        WHERE user_low_id = @UserLowId
                        AND user_high_id = @UserHighId
                        AND is_deleted = false
                    );
                    """,
                    new { UserLowId = userLowId, UserHighId = userHighId },
                    cancellationToken: cancellationToken));
        }

        private async Task<Conversation?> LoadDirectConversation(
            int userLowId,
            int userHighId,
            CancellationToken cancellationToken)
        {
            return await _dbContext.DirectConversations
                .Where(dc => dc.UserLowId == userLowId && dc.UserHighId == userHighId && !dc.IsDeleted)
                .Select(dc => dc.Conversation)
                .FirstOrDefaultAsync(c => !c.IsDeleted, cancellationToken);
        }

        private static bool IsUniqueViolation(DbUpdateException exception)
        {
            return exception.InnerException is PostgresException postgresException &&
                postgresException.SqlState == UniqueViolation;
        }
    }
}
