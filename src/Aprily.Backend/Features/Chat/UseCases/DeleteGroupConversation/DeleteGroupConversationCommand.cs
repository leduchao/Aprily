using Aprily.Backend.Common.Results;
using Aprily.Backend.Database;
using Aprily.Backend.Features.Chat.Hubs;
using Aprily.Backend.Features.Users.Services;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Aprily.Backend.Features.Chat.UseCases.DeleteGroupConversation;

public record DeleteGroupConversationResponse(Guid ConversationId);

public record DeleteGroupConversationCommand(Guid ConversationId)
    : IRequest<Result<DeleteGroupConversationResponse>>;

public sealed class DeleteGroupConversationCommandHandler(AppDbContext dbContext, ICurrentUser currentUser, IHubContext<ChatHub> chatHub)
    : IRequestHandler<DeleteGroupConversationCommand, Result<DeleteGroupConversationResponse>>
{
    public async Task<Result<DeleteGroupConversationResponse>> Handle(DeleteGroupConversationCommand request, CancellationToken cancellationToken)
    {
        var group = await dbContext.GroupConversations
            .Include(candidate => candidate.Conversation)
            .FirstOrDefaultAsync(candidate => candidate.Conversation.EntityId == request.ConversationId &&
                !candidate.IsDeleted && !candidate.Conversation.IsDeleted &&
                candidate.Conversation.ConversationMembers.Any(member => member.User.EntityId == currentUser.UserEntityId &&
                    !member.IsDeleted && member.Role == "owner"), cancellationToken);

        if (group is null)
            return Result<DeleteGroupConversationResponse>.Failure(new Error("chat.group_forbidden", "Only the group owner can dissolve this group"));

        var memberGroups = await dbContext.ConversationMembers
            .Where(member => member.ConversationId == group.ConversationId && !member.IsDeleted && !member.User.IsDeleted)
            .Select(member => ChatHub.UserGroup(member.User.EntityId))
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;

        group.IsDeleted = true;
        group.Conversation.IsDeleted = true;

        await dbContext.ConversationMembers
            .Where(member => member.ConversationId == group.ConversationId && !member.IsDeleted)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(member => member.IsDeleted, true)
                .SetProperty(member => member.UpdatedAt, now), cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        await chatHub.Clients.Groups(memberGroups).SendAsync("conversationUpdated", group.Conversation.EntityId, cancellationToken);

        return Result<DeleteGroupConversationResponse>.Success(new(group.Conversation.EntityId));
    }
}
