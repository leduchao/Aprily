using Aprily.Backend.Common.Extensions;

using MediatR;

namespace Aprily.Backend.Features.Chat.UseCases.SearchConversations;

public static class SearchConversationsEndpoint
{
    public static void MapSearchConversationsEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/conversations/search", async (
            string query,
            int? take,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var searchQuery = new SearchConversationsQuery(query, take ?? 20);
            var result = await sender.Send(searchQuery, cancellationToken);

            return result.ToHttpResult();
        });
    }
}
