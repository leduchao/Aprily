using Aprily.Backend.Common.Results;
using Aprily.Backend.Database;
using Aprily.Backend.Features.Chat.Hubs;
using Aprily.Backend.Features.Users.Services;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Aprily.Backend.Features.Chat.UseCases.RemoveGroupMember;

public record RemoveGroupMemberResponse(Guid ConversationId, Guid RemovedUserId);

public record RemoveGroupMemberCommand(Guid ConversationId, Guid MemberUserId)
    : IRequest<Result<RemoveGroupMemberResponse>>;

public sealed class RemoveGroupMemberCommandHandler(AppDbContext dbContext, ICurrentUser currentUser, IHubContext<ChatHub> chatHub)
    : IRequestHandler<RemoveGroupMemberCommand, Result<RemoveGroupMemberResponse>>
{
    public async Task<Result<RemoveGroupMemberResponse>> Handle(RemoveGroupMemberCommand request, CancellationToken cancellationToken)
    {
        var actorMembership = await dbContext.ConversationMembers
            .Include(member => member.User)
            .Include(member => member.Conversation)
            .FirstOrDefaultAsync(member => member.Conversation.EntityId == request.ConversationId &&
                member.Conversation.Type == "group" && !member.Conversation.IsDeleted &&
                !member.IsDeleted && member.User.EntityId == currentUser.UserEntityId &&
                (member.Role == "owner" || member.Role == "admin"), cancellationToken);

        if (actorMembership is null)
            return Result<RemoveGroupMemberResponse>.Failure(new Error("chat.group_forbidden", "You cannot remove members from this group"));

        var targetMembership = await dbContext.ConversationMembers
            .Include(member => member.User)
            .FirstOrDefaultAsync(member => member.ConversationId == actorMembership.ConversationId &&
                member.User.EntityId == request.MemberUserId && !member.IsDeleted && !member.User.IsDeleted, cancellationToken);

        if (targetMembership is null)
            return Result<RemoveGroupMemberResponse>.Failure(new Error("chat.group_member_not_found", "Group member not found"));

        if (targetMembership.Role == "owner")
            return Result<RemoveGroupMemberResponse>.Failure(new Error("chat.group_owner_required", "The group owner cannot be removed"));

        if (actorMembership.Role == "admin" && targetMembership.Role == "admin")
            return Result<RemoveGroupMemberResponse>.Failure(new Error("chat.group_forbidden", "Admins cannot remove other admins"));

        targetMembership.IsDeleted = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        var remainingGroups = await dbContext.ConversationMembers
            .Where(member => member.ConversationId == actorMembership.ConversationId && !member.IsDeleted && !member.User.IsDeleted)
            .Select(member => ChatHub.UserGroup(member.User.EntityId))
            .ToListAsync(cancellationToken);

        remainingGroups.Add(ChatHub.UserGroup(targetMembership.User.EntityId));
        await chatHub.Clients.Groups(remainingGroups).SendAsync("conversationUpdated", actorMembership.Conversation.EntityId, cancellationToken);

        return Result<RemoveGroupMemberResponse>.Success(new(actorMembership.Conversation.EntityId, targetMembership.User.EntityId));
    }
}
