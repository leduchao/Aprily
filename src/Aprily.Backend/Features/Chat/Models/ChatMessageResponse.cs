namespace Aprily.Backend.Features.Chat.Models;

public record ChatMessageResponse(
    Guid Id,
    Guid ConversationId,
    Guid SenderUserId,
    string SenderUsername,
    string? SenderAvatarUrl,
    string? Content,
    IReadOnlyList<ChatMessageAttachmentResponse> Attachments,
    DateTime SentAt,
    bool IsMine);
