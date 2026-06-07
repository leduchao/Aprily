using Aprily.Application.Abstractions.Repositories;
using Aprily.Application.Chat;
using Aprily.Domain.Entities;
using Aprily.Infrastructure.Database;

using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace Aprily.Infrastructure.Repositories;

public sealed class ChatRepository(AppDbContext db) : IChatRepository
{
    private const int DirectThreadType = 0;
    private readonly AppDbContext _db = db;

    public async Task<ChatMessageResponse> SendDirectMessage(
        Guid senderUserId,
        Guid recipientUserId,
        string content,
        CancellationToken cancellationToken)
    {
        User? sender = await _db.Users.SingleOrDefaultAsync(
            user => user.EntityId == senderUserId && !user.IsDeleted,
            cancellationToken);
        User? recipient = await _db.Users.SingleOrDefaultAsync(
            user => user.EntityId == recipientUserId && !user.IsDeleted,
            cancellationToken);

        if (sender is null)
        {
            throw new InvalidOperationException("The authenticated user no longer exists.");
        }

        if (recipient is null)
        {
            throw new KeyNotFoundException("The recipient user was not found.");
        }

        string conversationKey = CreateDirectConversationKey(sender.Id, recipient.Id);
        Domain.Entities.Thread? thread = await _db.Threads.SingleOrDefaultAsync(
            item => item.DirectConversationKey == conversationKey && !item.IsDeleted,
            cancellationToken);

        if (thread is null)
        {
            thread = new Domain.Entities.Thread
            {
                Type = DirectThreadType,
                DirectConversationKey = conversationKey,
                CreatorId = sender.Id,
                Participants =
                [
                    new ThreadParticipant { UserId = sender.Id },
                    new ThreadParticipant { UserId = recipient.Id }
                ]
            };

            _db.Threads.Add(thread);
        }

        var message = new Message
        {
            Content = content,
            SenderId = sender.Id,
            Thread = thread
        };

        _db.Messages.Add(message);
        thread.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation,
                ConstraintName: "IX_Threads_DirectConversationKey"
            })
        {
            // Both users can send the first message at once. Keep the thread that won the race.
            _db.ChangeTracker.Clear();
            thread = await _db.Threads.SingleAsync(
                item => item.DirectConversationKey == conversationKey && !item.IsDeleted,
                cancellationToken);
            message = new Message
            {
                Content = content,
                SenderId = sender.Id,
                ThreadId = thread.Id
            };
            _db.Messages.Add(message);
            thread.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(cancellationToken);
        }

        return MapMessage(message, thread, senderUserId, recipientUserId);
    }

    public async Task<IReadOnlyList<ChatMessageResponse>> GetDirectMessages(
        Guid currentUserId,
        Guid otherUserId,
        int take,
        DateTime? before,
        CancellationToken cancellationToken)
    {
        var userIds = await _db.Users
            .Where(user =>
                (user.EntityId == currentUserId || user.EntityId == otherUserId) &&
                !user.IsDeleted)
            .Select(user => new { user.Id, user.EntityId })
            .ToListAsync(cancellationToken);

        if (userIds.Count != 2)
        {
            throw new KeyNotFoundException("One or both users were not found.");
        }

        int currentDatabaseId = userIds.Single(user => user.EntityId == currentUserId).Id;
        int otherDatabaseId = userIds.Single(user => user.EntityId == otherUserId).Id;
        string conversationKey = CreateDirectConversationKey(currentDatabaseId, otherDatabaseId);

        IQueryable<Message> query = _db.Messages
            .AsNoTracking()
            .Where(message =>
                message.Thread!.DirectConversationKey == conversationKey &&
                !message.IsDeleted);

        if (before.HasValue)
        {
            query = query.Where(message => message.CreatedAt < before.Value);
        }

        return await query
            .OrderByDescending(message => message.CreatedAt)
            .Take(take)
            .Select(message => new ChatMessageResponse(
                message.EntityId,
                message.Thread!.EntityId,
                message.Sender!.EntityId,
                message.SenderId == currentDatabaseId ? otherUserId : currentUserId,
                message.Content,
                message.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DirectConversationResponse>> GetDirectConversations(
        Guid currentUserId,
        int take,
        DateTime? before,
        CancellationToken cancellationToken)
    {
        int? currentDatabaseId = await _db.Users
            .Where(user => user.EntityId == currentUserId && !user.IsDeleted)
            .Select(user => (int?)user.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (currentDatabaseId is null)
        {
            throw new KeyNotFoundException("The authenticated user was not found.");
        }

        var query = _db.Threads
            .AsNoTracking()
            .Where(thread =>
                thread.Type == DirectThreadType &&
                !thread.IsDeleted &&
                thread.Participants.Any(participant => participant.UserId == currentDatabaseId) &&
                thread.Messages.Any(message => !message.IsDeleted));

        if (before.HasValue)
        {
            query = query.Where(thread => thread.UpdatedAt < before.Value);
        }

        return await query
            .OrderByDescending(thread => thread.UpdatedAt)
            .Take(take)
            .Select(thread => new
            {
                ThreadId = thread.EntityId,
                thread.UpdatedAt,
                OtherUser = thread.Participants
                    .Where(participant => participant.UserId != currentDatabaseId)
                    .Select(participant => participant.User!)
                    .Single(),
                LastMessage = thread.Messages
                    .Where(message => !message.IsDeleted)
                    .OrderByDescending(message => message.CreatedAt)
                    .First()
            })
            .Select(item => new DirectConversationResponse(
                item.ThreadId,
                item.OtherUser.EntityId,
                item.OtherUser.Username,
                item.OtherUser.FullName,
                item.OtherUser.AvatarUrl,
                new ChatMessageResponse(
                    item.LastMessage.EntityId,
                    item.ThreadId,
                    item.LastMessage.Sender!.EntityId,
                    item.LastMessage.SenderId == currentDatabaseId
                        ? item.OtherUser.EntityId
                        : currentUserId,
                    item.LastMessage.Content,
                    item.LastMessage.CreatedAt),
                item.UpdatedAt))
            .ToListAsync(cancellationToken);
    }

    private static string CreateDirectConversationKey(int firstUserId, int secondUserId)
    {
        int lowerId = Math.Min(firstUserId, secondUserId);
        int higherId = Math.Max(firstUserId, secondUserId);
        return $"{lowerId}:{higherId}";
    }

    private static ChatMessageResponse MapMessage(
        Message message,
        Domain.Entities.Thread thread,
        Guid senderUserId,
        Guid recipientUserId)
    {
        return new ChatMessageResponse(
            message.EntityId,
            thread.EntityId,
            senderUserId,
            recipientUserId,
            message.Content,
            message.CreatedAt);
    }
}
