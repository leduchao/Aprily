using Aprily.Backend.Common.Results;
using Aprily.Backend.Database;
using Aprily.Backend.Features.Users.Services;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Aprily.Backend.Features.Chat.UseCases.MarkConversationAsRead;

public record MarkConversationAsReadResponse(
    Guid ConversationId,
    Guid LastReadMessageId,
    DateTime LastReadAt);

public sealed class MarkConversationAsReadCommand(Guid conversationId, Guid messageId)
    : IRequest<Result<MarkConversationAsReadResponse>>
{
    public Guid ConversationId { get; init; } = conversationId;
    public Guid MessageId { get; init; } = messageId;

    public sealed class Handler(AppDbContext dbContext, ICurrentUser currentUser)
        : IRequestHandler<MarkConversationAsReadCommand, Result<MarkConversationAsReadResponse>>
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ICurrentUser _currentUser = currentUser;

        public async Task<Result<MarkConversationAsReadResponse>> Handle(
            MarkConversationAsReadCommand request,
            CancellationToken cancellationToken)
        {
            var membership = await _dbContext.ConversationMembers
                .Include(cm => cm.Conversation)
                .Include(cm => cm.User)
                .FirstOrDefaultAsync(
                    cm => cm.Conversation.EntityId == request.ConversationId &&
                        cm.User.EntityId == _currentUser.UserEntityId &&
                        !cm.Conversation.IsDeleted &&
                        !cm.User.IsDeleted &&
                        !cm.IsDeleted,
                    cancellationToken);

            if (membership is null)
            {
                return Result<MarkConversationAsReadResponse>.Failure(
                    new Error("chat.conversation_not_found", "Conversation not found"));
            }

            var message = await _dbContext.Messages
                .FirstOrDefaultAsync(
                    m => m.EntityId == request.MessageId &&
                        m.ConversationId == membership.ConversationId &&
                        !m.IsDeleted,
                    cancellationToken);

            if (message is null)
            {
                return Result<MarkConversationAsReadResponse>.Failure(
                    new Error("chat.message_not_found", "Message not found"));
            }

            var now = DateTime.UtcNow;

            membership.LastReadMessageId = message.Id;
            membership.LastReadAt = now;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<MarkConversationAsReadResponse>.Success(
                new MarkConversationAsReadResponse(
                    membership.Conversation.EntityId,
                    message.EntityId,
                    membership.LastReadAt.Value));
        }
    }
}
