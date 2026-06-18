namespace Aprily.Backend.Features.Friends.Models;

public record FriendResponse(
    Guid Id,
    FriendUserResponse User,
    DateTime CreatedAt);
