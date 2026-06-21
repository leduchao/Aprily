namespace Aprily.Backend.Features.Chat.Models;

public record ChatMessageAttachmentResponse(
    Guid Id,
    string Type,
    string Url,
    string? OriginalFileName,
    string ContentType,
    long SizeBytes,
    int? Width,
    int? Height,
    short SortOrder);
