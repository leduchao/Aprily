namespace Aprily.Backend.Features.Friends.Models;

public record FriendUserResponse(
    Guid Id,
    string Username,
    string? FullName,
    string Email,
    string? AvatarUrl);
