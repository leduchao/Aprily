namespace Aprily.Application.Chat;

public sealed record DirectConversationResponse(
    Guid ThreadId,
    Guid OtherUserId,
    string OtherUsername,
    string? OtherFullName,
    string? OtherAvatarUrl,
    ChatMessageResponse LastMessage,
    DateTime UpdatedAt);
