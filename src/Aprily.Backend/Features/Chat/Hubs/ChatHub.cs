using Aprily.Backend.Common.Extensions;
using Aprily.Backend.Database.Connection;

using Dapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Aprily.Backend.Features.Chat.Hubs;

[Authorize]
public sealed class ChatHub(IDbConnectionFactory dbConnectionFactory) : Hub
{
    public const string HubPath = "/hubs/chat";

    private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;

    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(GetCurrentUserId()));
        await base.OnConnectedAsync();
    }

    public async Task JoinConversation(Guid conversationId)
    {
        if (!await IsConversationMember(conversationId))
        {
            throw new HubException("Conversation not found");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, ConversationGroup(conversationId));
    }

    public async Task LeaveConversation(Guid conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, ConversationGroup(conversationId));
    }

    public static string UserGroup(Guid userId) => $"user:{userId}";

    public static string ConversationGroup(Guid conversationId) => $"conversation:{conversationId}";

    private async Task<bool> IsConversationMember(Guid conversationId)
    {
        using var conn = await _dbConnectionFactory.CreateConnection();

        return await conn.ExecuteScalarAsync<bool>(
            """
            SELECT EXISTS (
                SELECT 1
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
            );
            """,
            new
            {
                ConversationId = conversationId,
                CurrentUserId = GetCurrentUserId()
            });
    }

    private Guid GetCurrentUserId()
    {
        return Context.User?.GetUserEntityId()
            ?? throw new HubException("User is not authenticated");
    }
}
