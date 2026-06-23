using Aprily.Backend.Common.Results;
using Aprily.Backend.Database;
using Aprily.Backend.Entities;
using Aprily.Backend.Features.Chat.Hubs;
using Aprily.Backend.Features.Users.Services;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Aprily.Backend.Features.Chat.UseCases.CreateGroupConversation;

public record CreateGroupConversationResponse(Guid ConversationId);

public sealed class CreateGroupConversationCommand(string name, IReadOnlyList<Guid> memberUserIds)
    : IRequest<Result<CreateGroupConversationResponse>>
{
    public string Name { get; } = name;
    public IReadOnlyList<Guid> MemberUserIds { get; } = memberUserIds;

    public sealed class Handler(AppDbContext dbContext, ICurrentUser currentUser, IHubContext<ChatHub> chatHub)
        : IRequestHandler<CreateGroupConversationCommand, Result<CreateGroupConversationResponse>>
    {
        public async Task<Result<CreateGroupConversationResponse>> Handle(CreateGroupConversationCommand request, CancellationToken cancellationToken)
        {
            var owner = await dbContext.Users.FirstOrDefaultAsync(u => u.EntityId == currentUser.UserEntityId && !u.IsDeleted, cancellationToken);
            if (owner is null)
                return Result<CreateGroupConversationResponse>.Failure(new Error("users.user_notFound", "User not found"));

            var requestedIds = request.MemberUserIds.Distinct().Where(id => id != owner.EntityId).ToArray();
            var members = await dbContext.Users.Where(u => requestedIds.Contains(u.EntityId) && !u.IsDeleted).ToListAsync(cancellationToken);
            if (members.Count != requestedIds.Length)
                return Result<CreateGroupConversationResponse>.Failure(new Error("chat.group_member_not_found", "One or more members were not found"));

            var memberInternalIds = members.Select(u => u.Id).ToArray();
            var friendCount = await dbContext.Friendships.CountAsync(f =>
                !f.IsDeleted && ((f.UserLowId == owner.Id && memberInternalIds.Contains(f.UserHighId)) ||
                (f.UserHighId == owner.Id && memberInternalIds.Contains(f.UserLowId))), cancellationToken);
            if (friendCount != members.Count)
                return Result<CreateGroupConversationResponse>.Failure(new Error("chat.group_members_not_friends", "You can only add friends to a group"));

            var conversation = new Conversation { EntityId = Guid.NewGuid(), Type = "group", IsDeleted = false };
            var group = new GroupConversation
            {
                EntityId = Guid.NewGuid(), Conversation = conversation, Name = request.Name.Trim(),
                CreatedByUserId = owner.Id, CreatedByUser = owner, IsDeleted = false
            };
            conversation.ConversationMembers.Add(new ConversationMember
            {
                EntityId = Guid.NewGuid(), UserId = owner.Id, Role = "owner", IsDeleted = false
            });
            foreach (var member in members)
                conversation.ConversationMembers.Add(new ConversationMember
                {
                    EntityId = Guid.NewGuid(), UserId = member.Id, Role = "member", IsDeleted = false
                });

            await dbContext.GroupConversations.AddAsync(group, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            var userIds = members.Select(u => u.EntityId).Append(owner.EntityId).Select(ChatHub.UserGroup).ToArray();
            await chatHub.Clients.Groups(userIds).SendAsync("conversationUpdated", conversation.EntityId, cancellationToken);
            return Result<CreateGroupConversationResponse>.Success(new(conversation.EntityId));
        }
    }
}
