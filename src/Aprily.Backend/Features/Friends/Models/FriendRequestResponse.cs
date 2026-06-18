namespace Aprily.Backend.Features.Friends.Models;

public record FriendRequestResponse(
    Guid Id,
    FriendUserResponse Requester,
    FriendUserResponse Addressee,
    string Status,
    DateTime CreatedAt,
    DateTime? RespondedAt);
