namespace Aprily.Backend.Features.Chat.Models;

public record LastMessageResponse(
    Guid Id,
    Guid SenderUserId,
    string? Content,
    bool HasAttachments,
    DateTime SentAt);
