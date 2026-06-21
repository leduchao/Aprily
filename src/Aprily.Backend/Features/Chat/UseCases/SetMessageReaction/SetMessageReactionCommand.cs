using Aprily.Backend.Common.Results;
using Aprily.Backend.Database;
using Aprily.Backend.Entities;
using Aprily.Backend.Features.Chat.Hubs;
using Aprily.Backend.Features.Chat.Models;
using Aprily.Backend.Features.Users.Services;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Aprily.Backend.Features.Chat.UseCases.SetMessageReaction;

public sealed record SetMessageReactionCommand(Guid MessageId, string? Type)
    : IRequest<Result<MessageReactionsUpdatedResponse>>;

public sealed class SetMessageReactionCommandHandler(
    AppDbContext dbContext,
    ICurrentUser currentUser,
    IHubContext<ChatHub> chatHub)
    : IRequestHandler<SetMessageReactionCommand, Result<MessageReactionsUpdatedResponse>>
{
    public async Task<Result<MessageReactionsUpdatedResponse>> Handle(
        SetMessageReactionCommand request,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(
            candidate => candidate.EntityId == currentUser.UserEntityId && !candidate.IsDeleted,
            cancellationToken);

        if (user is null)
        {
            return Result<MessageReactionsUpdatedResponse>.Failure(
                new Error("users.user_notFound", "User not found"));
        }

        var message = await dbContext.Messages
            .Include(candidate => candidate.Conversation)
            .FirstOrDefaultAsync(
                candidate =>
                    candidate.EntityId == request.MessageId &&
                    !candidate.IsDeleted &&
                    !candidate.Conversation.IsDeleted &&
                    candidate.Conversation.ConversationMembers.Any(member =>
                        member.UserId == user.Id && !member.IsDeleted),
                cancellationToken);

        if (message is null)
        {
            return Result<MessageReactionsUpdatedResponse>.Failure(
                new Error("chat.message_not_found", "Message not found"));
        }

        var existingReaction = await dbContext.MessageReactions.FirstOrDefaultAsync(
            reaction =>
                reaction.MessageId == message.Id &&
                reaction.UserId == user.Id &&
                !reaction.IsDeleted,
            cancellationToken);

        if (request.Type is null)
        {
            if (existingReaction is not null)
            {
                existingReaction.IsDeleted = true;
            }
        }
        else if (existingReaction is not null)
        {
            existingReaction.Type = request.Type;
        }
        else
        {
            await dbContext.MessageReactions.AddAsync(new MessageReaction
            {
                EntityId = Guid.NewGuid(),
                MessageId = message.Id,
                UserId = user.Id,
                Type = request.Type,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            }, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var reactionCounts = await dbContext.MessageReactions
            .Where(reaction => reaction.MessageId == message.Id && !reaction.IsDeleted)
            .GroupBy(reaction => reaction.Type)
            .Select(group => new
            {
                Type = group.Key,
                Count = group.Count()
            })
            .OrderBy(reaction => reaction.Type)
            .ToListAsync(cancellationToken);

        var reactions = reactionCounts
            .Select(reaction => new MessageReactionSummaryResponse(
                reaction.Type,
                reaction.Count,
                reaction.Type == request.Type))
            .ToList();

        var response = new MessageReactionsUpdatedResponse(
            message.Conversation.EntityId,
            message.EntityId,
            user.EntityId,
            reactions);

        var memberGroups = await dbContext.ConversationMembers
            .Where(member => member.ConversationId == message.ConversationId && !member.IsDeleted)
            .Select(member => ChatHub.UserGroup(member.User.EntityId))
            .ToListAsync(cancellationToken);

        await chatHub.Clients
            .Groups(memberGroups)
            .SendAsync("messageReactionsUpdated", response, cancellationToken);

        return Result<MessageReactionsUpdatedResponse>.Success(response);
    }
}
