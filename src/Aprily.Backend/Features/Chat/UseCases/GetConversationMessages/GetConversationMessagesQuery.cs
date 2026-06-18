using Aprily.Backend.Common.Results;
using Aprily.Backend.Database.Connection;
using Aprily.Backend.Features.Chat.Models;
using Aprily.Backend.Features.Users.Services;

using Dapper;

using MediatR;

namespace Aprily.Backend.Features.Chat.UseCases.GetConversationMessages;

public sealed class GetConversationMessagesQuery(Guid conversationId, int take, DateTime? before)
    : IRequest<Result<IReadOnlyList<ChatMessageResponse>>>
{
    public Guid ConversationId { get; init; } = conversationId;
    public int Take { get; init; } = take;
    public DateTime? Before { get; init; } = before;

    public sealed class Handler(IDbConnectionFactory dbConnectionFactory, ICurrentUser currentUser)
        : IRequestHandler<GetConversationMessagesQuery, Result<IReadOnlyList<ChatMessageResponse>>>
    {
        private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;
        private readonly ICurrentUser _currentUser = currentUser;

        public async Task<Result<IReadOnlyList<ChatMessageResponse>>> Handle(
            GetConversationMessagesQuery request,
            CancellationToken cancellationToken)
        {
            using var conn = await _dbConnectionFactory.CreateConnection();

            var membershipSql = """
                SELECT c.id
                FROM conversations c
                INNER JOIN conversation_members cm
                    ON cm.conversation_id = c.id
                    AND cm.is_deleted = false
                INNER JOIN users u
                    ON u.id = cm.user_id
                    AND u.is_deleted = false
                WHERE c.entity_id = @ConversationId
                AND c.is_deleted = false
                AND u.entity_id = @CurrentUserId
                LIMIT 1;
                """;

            var conversationInternalId = await conn.QueryFirstOrDefaultAsync<int?>(
                new CommandDefinition(
                    membershipSql,
                    new
                    {
                        request.ConversationId,
                        CurrentUserId = _currentUser.UserEntityId
                    },
                    cancellationToken: cancellationToken));

            if (conversationInternalId is null)
            {
                return Result<IReadOnlyList<ChatMessageResponse>>.Failure(
                    new Error("chat.conversation_not_found", "Conversation not found"));
            }

            var sql = """
                SELECT
                    m.entity_id AS Id,
                    c.entity_id AS ConversationId,
                    sender.entity_id AS SenderUserId,
                    sender.username AS SenderUsername,
                    sender.avatar_url AS SenderAvatarUrl,
                    m.content AS Content,
                    m.sent_at AS SentAt,
                    (sender.entity_id = @CurrentUserId) AS IsMine
                FROM messages m
                INNER JOIN conversations c
                    ON c.id = m.conversation_id
                    AND c.is_deleted = false
                INNER JOIN users sender
                    ON sender.id = m.sender_user_id
                    AND sender.is_deleted = false
                WHERE m.conversation_id = @ConversationInternalId
                AND m.is_deleted = false
                AND (@Before::timestamptz IS NULL OR m.sent_at < @Before::timestamptz)
                ORDER BY m.sent_at DESC, m.id DESC
                LIMIT @Take;
                """;

            var messages = await conn.QueryAsync<ChatMessageResponse>(
                new CommandDefinition(
                    sql,
                    new
                    {
                        ConversationInternalId = conversationInternalId.Value,
                        CurrentUserId = _currentUser.UserEntityId,
                        request.Before,
                        request.Take
                    },
                    cancellationToken: cancellationToken));

            return Result<IReadOnlyList<ChatMessageResponse>>.Success(messages.ToList());
        }
    }
}
