namespace Aprily.Backend.Features.Chat.Models;

public record ChatMessageReplyResponse(
    Guid Id,
    Guid SenderUserId,
    string SenderUsername,
    string? Content,
    bool HasAttachments);
