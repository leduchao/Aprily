using Aprily.Backend.Common.Results;
using Aprily.Backend.Database;
using Aprily.Backend.Features.Chat.Hubs;
using Aprily.Backend.Features.Users.Services;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Aprily.Backend.Features.Chat.UseCases.LeaveGroupConversation;

public record LeaveGroupConversationResponse(Guid ConversationId);

public record LeaveGroupConversationCommand(Guid ConversationId)
    : IRequest<Result<LeaveGroupConversationResponse>>;

public sealed class LeaveGroupConversationCommandHandler(AppDbContext dbContext, ICurrentUser currentUser, IHubContext<ChatHub> chatHub)
    : IRequestHandler<LeaveGroupConversationCommand, Result<LeaveGroupConversationResponse>>
{
    public async Task<Result<LeaveGroupConversationResponse>> Handle(LeaveGroupConversationCommand request, CancellationToken cancellationToken)
    {
        var membership = await dbContext.ConversationMembers
            .Include(member => member.User)
            .Include(member => member.Conversation)
            .FirstOrDefaultAsync(member => member.Conversation.EntityId == request.ConversationId &&
                member.Conversation.Type == "group" && !member.Conversation.IsDeleted &&
                member.User.EntityId == currentUser.UserEntityId && !member.IsDeleted && !member.User.IsDeleted, cancellationToken);

        if (membership is null)
            return Result<LeaveGroupConversationResponse>.Failure(new Error("chat.group_not_found", "Group conversation not found"));

        if (membership.Role == "owner")
            return Result<LeaveGroupConversationResponse>.Failure(new Error("chat.group_owner_required", "The group owner must dissolve the group instead"));

        membership.IsDeleted = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        var memberGroups = await dbContext.ConversationMembers
            .Where(member => member.ConversationId == membership.ConversationId && !member.IsDeleted && !member.User.IsDeleted)
            .Select(member => ChatHub.UserGroup(member.User.EntityId))
            .ToListAsync(cancellationToken);

        memberGroups.Add(ChatHub.UserGroup(membership.User.EntityId));
        await chatHub.Clients.Groups(memberGroups).SendAsync("conversationUpdated", membership.Conversation.EntityId, cancellationToken);

        return Result<LeaveGroupConversationResponse>.Success(new(membership.Conversation.EntityId));
    }
}
