using Aprily.Backend.Common.Results;
using Aprily.Backend.Database;
using Aprily.Backend.Entities;
using Aprily.Backend.Features.Chat.Hubs;
using Aprily.Backend.Features.Users.Services;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Aprily.Backend.Features.Chat.UseCases.AddGroupMembers;

public record AddGroupMembersResponse(Guid ConversationId, int AddedCount);
public record AddGroupMembersCommand(Guid ConversationId, IReadOnlyList<Guid> MemberUserIds)
    : IRequest<Result<AddGroupMembersResponse>>;

public sealed class AddGroupMembersCommandHandler(AppDbContext dbContext, ICurrentUser currentUser, IHubContext<ChatHub> chatHub)
    : IRequestHandler<AddGroupMembersCommand, Result<AddGroupMembersResponse>>
{
    public async Task<Result<AddGroupMembersResponse>> Handle(AddGroupMembersCommand request, CancellationToken cancellationToken)
    {
        var actorMembership = await dbContext.ConversationMembers.Include(member => member.User).Include(member => member.Conversation)
            .FirstOrDefaultAsync(member => member.Conversation.EntityId == request.ConversationId &&
                member.Conversation.Type == "group" && !member.Conversation.IsDeleted && !member.IsDeleted &&
                member.User.EntityId == currentUser.UserEntityId && (member.Role == "owner" || member.Role == "admin"), cancellationToken);
        if (actorMembership is null)
            return Result<AddGroupMembersResponse>.Failure(new Error("chat.group_forbidden", "You cannot add members to this group"));

        var requestedIds = request.MemberUserIds.Distinct().ToArray();
        var users = await dbContext.Users.Where(user => requestedIds.Contains(user.EntityId) && !user.IsDeleted).ToListAsync(cancellationToken);
        if (users.Count != requestedIds.Length)
            return Result<AddGroupMembersResponse>.Failure(new Error("chat.group_member_not_found", "One or more users were not found"));

        var userInternalIds = users.Select(user => user.Id).ToArray();
        var existingIds = await dbContext.ConversationMembers.Where(member => member.ConversationId == actorMembership.ConversationId &&
            userInternalIds.Contains(member.UserId) && !member.IsDeleted).Select(member => member.UserId).ToListAsync(cancellationToken);
        var usersToAdd = users.Where(user => !existingIds.Contains(user.Id)).ToList();

        var usersToAddIds = usersToAdd.Select(user => user.Id).ToArray();
        var friendCount = await dbContext.Friendships.CountAsync(friendship => !friendship.IsDeleted &&
            ((friendship.UserLowId == actorMembership.UserId && usersToAddIds.Contains(friendship.UserHighId)) ||
             (friendship.UserHighId == actorMembership.UserId && usersToAddIds.Contains(friendship.UserLowId))), cancellationToken);
        if (friendCount != usersToAdd.Count)
            return Result<AddGroupMembersResponse>.Failure(new Error("chat.group_members_not_friends", "You can only add your friends"));

        await dbContext.ConversationMembers.AddRangeAsync(usersToAdd.Select(user => new ConversationMember
        {
            EntityId = Guid.NewGuid(), ConversationId = actorMembership.ConversationId,
            UserId = user.Id, Role = "member", IsDeleted = false
        }), cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var allGroups = await dbContext.ConversationMembers.Where(member => member.ConversationId == actorMembership.ConversationId && !member.IsDeleted && !member.User.IsDeleted)
            .Select(member => ChatHub.UserGroup(member.User.EntityId)).ToListAsync(cancellationToken);
        await chatHub.Clients.Groups(allGroups).SendAsync("conversationUpdated", actorMembership.Conversation.EntityId, cancellationToken);
        return Result<AddGroupMembersResponse>.Success(new(actorMembership.Conversation.EntityId, usersToAdd.Count));
    }
}
