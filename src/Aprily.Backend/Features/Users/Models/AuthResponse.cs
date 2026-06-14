namespace Aprily.Backend.Features.Users.Models;

public record UserBasicInfo(
    Guid Id,
    string Username,
    string? FullName,
    string Email,
    string? AvatarUrl,
    DateTime LastLoginAt,
    bool IsEmailVerified);
