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
                    m.id AS InternalId,
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

            var messageRows = (await conn.QueryAsync<MessageRow>(
                new CommandDefinition(
                    sql,
                    new
                    {
                        ConversationInternalId = conversationInternalId.Value,
                        CurrentUserId = _currentUser.UserEntityId,
                        request.Before,
                        request.Take
                    },
                    cancellationToken: cancellationToken))).ToList();

            var messageIds = messageRows.Select(message => message.InternalId).ToArray();
            var attachmentRows = messageIds.Length == 0
                ? []
                : (await conn.QueryAsync<AttachmentRow>(
                    new CommandDefinition(
                        """
                        SELECT
                            ma.message_id AS MessageId,
                            ma.entity_id AS Id,
                            ma.type AS Type,
                            ma.url AS Url,
                            ma.original_file_name AS OriginalFileName,
                            ma.content_type AS ContentType,
                            ma.size_bytes AS SizeBytes,
                            ma.width AS Width,
                            ma.height AS Height,
                            ma.sort_order AS SortOrder
                        FROM message_attachments ma
                        WHERE ma.message_id = ANY(@MessageIds)
                        AND ma.is_deleted = false
                        ORDER BY ma.message_id, ma.sort_order;
                        """,
                        new { MessageIds = messageIds },
                        cancellationToken: cancellationToken))).ToList();

            var attachmentsByMessageId = attachmentRows
                .GroupBy(attachment => attachment.MessageId)
                .ToDictionary(
                    group => group.Key,
                    group => (IReadOnlyList<ChatMessageAttachmentResponse>)group
                        .Select(attachment => new ChatMessageAttachmentResponse(
                            attachment.Id,
                            attachment.Type,
                            attachment.Url,
                            attachment.OriginalFileName,
                            attachment.ContentType,
                            attachment.SizeBytes,
                            attachment.Width,
                            attachment.Height,
                            attachment.SortOrder))
                        .ToList());

            var messages = messageRows
                .Select(message => new ChatMessageResponse(
                    message.Id,
                    message.ConversationId,
                    message.SenderUserId,
                    message.SenderUsername,
                    message.SenderAvatarUrl,
                    message.Content,
                    attachmentsByMessageId.GetValueOrDefault(message.InternalId, []),
                    message.SentAt,
                    message.IsMine))
                .ToList();

            return Result<IReadOnlyList<ChatMessageResponse>>.Success(messages);
        }

        private sealed class MessageRow
        {
            public int InternalId { get; init; }
            public Guid Id { get; init; }
            public Guid ConversationId { get; init; }
            public Guid SenderUserId { get; init; }
            public string SenderUsername { get; init; } = null!;
            public string? SenderAvatarUrl { get; init; }
            public string? Content { get; init; }
            public DateTime SentAt { get; init; }
            public bool IsMine { get; init; }
        }

        private sealed class AttachmentRow
        {
            public int MessageId { get; init; }
            public Guid Id { get; init; }
            public string Type { get; init; } = null!;
            public string Url { get; init; } = null!;
            public string? OriginalFileName { get; init; }
            public string ContentType { get; init; } = null!;
            public long SizeBytes { get; init; }
            public int? Width { get; init; }
            public int? Height { get; init; }
            public short SortOrder { get; init; }
        }
    }
}
