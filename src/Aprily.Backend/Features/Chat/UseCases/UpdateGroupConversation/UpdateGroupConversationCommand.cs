using Aprily.Backend.Common.Results;
using Aprily.Backend.Database;
using Aprily.Backend.Features.Chat.Hubs;
using Aprily.Backend.Features.Users.Services;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Aprily.Backend.Features.Chat.UseCases.UpdateGroupConversation;

public record UpdateGroupConversationResponse(Guid ConversationId, string Name);
public record UpdateGroupConversationCommand(Guid ConversationId, string Name)
    : IRequest<Result<UpdateGroupConversationResponse>>;

public sealed class UpdateGroupConversationCommandHandler(AppDbContext dbContext, ICurrentUser currentUser, IHubContext<ChatHub> chatHub)
    : IRequestHandler<UpdateGroupConversationCommand, Result<UpdateGroupConversationResponse>>
{
    public async Task<Result<UpdateGroupConversationResponse>> Handle(UpdateGroupConversationCommand request, CancellationToken cancellationToken)
    {
        var group = await dbContext.GroupConversations
            .Include(candidate => candidate.Conversation)
            .FirstOrDefaultAsync(candidate => candidate.Conversation.EntityId == request.ConversationId &&
                !candidate.IsDeleted && !candidate.Conversation.IsDeleted &&
                candidate.Conversation.ConversationMembers.Any(member => member.User.EntityId == currentUser.UserEntityId &&
                    !member.IsDeleted && (member.Role == "owner" || member.Role == "admin")), cancellationToken);

        if (group is null)
            return Result<UpdateGroupConversationResponse>.Failure(new Error("chat.group_forbidden", "You cannot edit this group"));

        group.Name = request.Name.Trim();
        await dbContext.SaveChangesAsync(cancellationToken);

        var memberGroups = await dbContext.ConversationMembers
            .Where(member => member.ConversationId == group.ConversationId && !member.IsDeleted && !member.User.IsDeleted)
            .Select(member => ChatHub.UserGroup(member.User.EntityId)).ToListAsync(cancellationToken);
        await chatHub.Clients.Groups(memberGroups).SendAsync("conversationUpdated", group.Conversation.EntityId, cancellationToken);

        return Result<UpdateGroupConversationResponse>.Success(new(group.Conversation.EntityId, group.Name));
    }
}
