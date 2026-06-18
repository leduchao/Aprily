namespace Aprily.Backend.Features.Chat.Models;

public record ConversationResponse(
    Guid Id,
    string Type,
    ChatUserResponse OtherUser,
    LastMessageResponse? LastMessage,
    DateTime? LastMessageAt,
    int UnreadCount);
