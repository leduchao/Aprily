using Aprily.Backend.Common.Constants;
using Aprily.Backend.Features.Friends.UseCases.RespondFriendRequest;
using Aprily.Backend.Features.Friends.UseCases.ListFriendRequests;
using Aprily.Backend.Features.Friends.UseCases.ListFriends;
using Aprily.Backend.Features.Friends.UseCases.SendFriendRequest;

namespace Aprily.Backend.Features.Friends;

public static class FriendsEndpoints
{
    public static void MapFriendsEndpoints(this IEndpointRouteBuilder app)
    {
        var friends = app
            .MapGroup($"{ApiPath.BasePath}/friends")
            .WithTags("Friends")
            .RequireAuthorization();

        friends.MapListFriendsEndpoint();
        friends.MapListFriendRequestsEndpoint();
        friends.MapSendFriendRequestEndpoint();
        friends.MapAcceptFriendRequestEndpoint();
        friends.MapDeclineFriendRequestEndpoint();
    }
}
