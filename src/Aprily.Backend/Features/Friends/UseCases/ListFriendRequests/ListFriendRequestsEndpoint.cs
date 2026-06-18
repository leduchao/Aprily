using Aprily.Backend.Common.Extensions;

using MediatR;

namespace Aprily.Backend.Features.Friends.UseCases.ListFriendRequests;

public static class ListFriendRequestsEndpoint
{
    public static void MapListFriendRequestsEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/requests", async (
            string? direction,
            string? status,
            int? take,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new ListFriendRequestsQuery(
                direction ?? "incoming",
                status ?? "pending",
                take ?? 50);

            var result = await sender.Send(query, cancellationToken);

            return result.ToHttpResult();
        });
    }
}
