using Aprily.Backend.Common.Extensions;

using MediatR;

namespace Aprily.Backend.Features.Friends.UseCases.ListFriends;

public static class ListFriendsEndpoint
{
    public static void MapListFriendsEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (
            int? take,
            DateTime? before,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new ListFriendsQuery(take ?? 50, before);
            var result = await sender.Send(query, cancellationToken);

            return result.ToHttpResult();
        });
    }
}
