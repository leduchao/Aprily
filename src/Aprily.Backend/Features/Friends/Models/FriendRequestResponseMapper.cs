using Aprily.Backend.Entities;

namespace Aprily.Backend.Features.Friends.Models;

internal static class FriendRequestResponseMapper
{
    public static FriendRequestResponse ToResponse(this FriendRequest request)
    {
        return new FriendRequestResponse(
            request.EntityId,
            new FriendUserResponse(
                request.RequesterUser.EntityId,
                request.RequesterUser.Username,
                request.RequesterUser.FullName,
                request.RequesterUser.Email,
                request.RequesterUser.AvatarUrl),
            new FriendUserResponse(
                request.AddresseeUser.EntityId,
                request.AddresseeUser.Username,
                request.AddresseeUser.FullName,
                request.AddresseeUser.Email,
                request.AddresseeUser.AvatarUrl),
            request.Status,
            request.CreatedAt,
            request.RespondedAt);
    }
}
