using Aprily.Backend.Common.Results;
using Aprily.Backend.Database.Connection;
using Aprily.Backend.Features.Chat.Models;
using Aprily.Backend.Features.Users.Services;

using Dapper;

using MediatR;

namespace Aprily.Backend.Features.Chat.UseCases.ListConversations;

public sealed class ListConversationsQuery(int take, DateTime? before)
    : IRequest<Result<IReadOnlyList<ConversationResponse>>>
{
    public int Take { get; init; } = take;
    public DateTime? Before { get; init; } = before;

    public sealed class Handler(IDbConnectionFactory dbConnectionFactory, ICurrentUser currentUser)
        : IRequestHandler<ListConversationsQuery, Result<IReadOnlyList<ConversationResponse>>>
    {
        private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;
        private readonly ICurrentUser _currentUser = currentUser;

        public async Task<Result<IReadOnlyList<ConversationResponse>>> Handle(
            ListConversationsQuery request,
            CancellationToken cancellationToken)
        {
            using var conn = await _dbConnectionFactory.CreateConnection();

            var sql = """
                WITH me AS (
                    SELECT id
                    FROM users
                    WHERE entity_id = @CurrentUserId
                    AND is_deleted = false
                    LIMIT 1
                )
                SELECT
                    c.entity_id AS ConversationId,
                    c.type AS Type,
                    c.last_message_at AS LastMessageAt,
                    ou.entity_id AS OtherUserId,
                    ou.username AS OtherUsername,
                    ou.full_name AS OtherFullName,
                    ou.avatar_url AS OtherAvatarUrl,
                    lm.entity_id AS LastMessageId,
                    lms.entity_id AS LastMessageSenderUserId,
                    lm.content AS LastMessageContent,
                    lm.sent_at AS LastMessageSentAt,
                    COALESCE(unread.unread_count, 0) AS UnreadCount
                FROM me cu
                INNER JOIN conversation_members cm
                    ON cm.user_id = cu.id
                    AND cm.is_deleted = false
                INNER JOIN conversations c
                    ON c.id = cm.conversation_id
                    AND c.is_deleted = false
                INNER JOIN conversation_members ocm
                    ON ocm.conversation_id = c.id
                    AND ocm.user_id <> cu.id
                    AND ocm.is_deleted = false
                INNER JOIN users ou
                    ON ou.id = ocm.user_id
                    AND ou.is_deleted = false
                LEFT JOIN messages lm
                    ON lm.id = c.last_message_id
                    AND lm.is_deleted = false
                LEFT JOIN users lms
                    ON lms.id = lm.sender_user_id
                LEFT JOIN LATERAL (
                    SELECT COUNT(*)::int AS unread_count
                    FROM messages m
                    WHERE m.conversation_id = c.id
                    AND m.is_deleted = false
                    AND m.sender_user_id <> cu.id
                    AND (
                        cm.last_read_message_id IS NULL
                        OR m.id > cm.last_read_message_id
                    )
                ) unread ON true
                WHERE (@Before::timestamptz IS NULL OR c.last_message_at < @Before::timestamptz)
                ORDER BY c.last_message_at DESC NULLS LAST, c.id DESC
                LIMIT @Take;
                """;

            var rows = await conn.QueryAsync<ConversationRow>(
                new CommandDefinition(
                    sql,
                    new
                    {
                        CurrentUserId = _currentUser.UserEntityId,
                        request.Before,
                        request.Take
                    },
                    cancellationToken: cancellationToken));

            var conversations = rows
                .Select(row => new ConversationResponse(
                    row.ConversationId,
                    row.Type,
                    new ChatUserResponse(
                        row.OtherUserId,
                        row.OtherUsername,
                        row.OtherFullName,
                        row.OtherAvatarUrl),
                    row.LastMessageId is null || row.LastMessageSenderUserId is null || row.LastMessageSentAt is null
                        ? null
                        : new LastMessageResponse(
                            row.LastMessageId.Value,
                            row.LastMessageSenderUserId.Value,
                            row.LastMessageContent ?? string.Empty,
                            row.LastMessageSentAt.Value),
                    row.LastMessageAt,
                    row.UnreadCount))
                .ToList();

            return Result<IReadOnlyList<ConversationResponse>>.Success(conversations);
        }

        private sealed class ConversationRow
        {
            public Guid ConversationId { get; init; }
            public string Type { get; init; } = null!;
            public DateTime? LastMessageAt { get; init; }
            public Guid OtherUserId { get; init; }
            public string OtherUsername { get; init; } = null!;
            public string? OtherFullName { get; init; }
            public string? OtherAvatarUrl { get; init; }
            public Guid? LastMessageId { get; init; }
            public Guid? LastMessageSenderUserId { get; init; }
            public string? LastMessageContent { get; init; }
            public DateTime? LastMessageSentAt { get; init; }
            public int UnreadCount { get; init; }
        }
    }
}
