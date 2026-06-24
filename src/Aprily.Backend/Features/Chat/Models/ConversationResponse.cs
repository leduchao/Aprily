namespace Aprily.Backend.Features.Chat.Models;

public record ConversationResponse(
    Guid Id,
    string Type,
    string Name,
    string? AvatarUrl,
    ChatUserResponse? OtherUser,
    int MemberCount,
    LastMessageResponse? LastMessage,
    DateTime? LastMessageAt,
    int UnreadCount);
