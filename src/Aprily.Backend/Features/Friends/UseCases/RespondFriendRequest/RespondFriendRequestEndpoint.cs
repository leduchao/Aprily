using Aprily.Backend.Common.Extensions;

using MediatR;

namespace Aprily.Backend.Features.Friends.UseCases.RespondFriendRequest;

public static class RespondFriendRequestEndpoint
{
    public static void MapAcceptFriendRequestEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/requests/{requestId:guid}/accept", async (
            Guid requestId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new RespondFriendRequestCommand(requestId, FriendRequestDecision.Accept);
            var result = await sender.Send(command, cancellationToken);

            return result.ToHttpResult();
        });
    }

    public static void MapDeclineFriendRequestEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/requests/{requestId:guid}/decline", async (
            Guid requestId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new RespondFriendRequestCommand(requestId, FriendRequestDecision.Decline);
            var result = await sender.Send(command, cancellationToken);

            return result.ToHttpResult();
        });
    }
}
