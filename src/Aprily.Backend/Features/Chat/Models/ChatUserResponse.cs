namespace Aprily.Backend.Features.Chat.Models;

public record ChatUserResponse(
    Guid Id,
    string Username,
    string? FullName,
    string? AvatarUrl);
