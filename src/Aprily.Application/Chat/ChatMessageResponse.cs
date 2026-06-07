namespace Aprily.Application.Chat;

public sealed record ChatMessageResponse(
    Guid MessageId,
    Guid ThreadId,
    Guid SenderUserId,
    Guid RecipientUserId,
    string Content,
    DateTime SentAt);
