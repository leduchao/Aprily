using Aprily.Backend.Common.Extensions;

using MediatR;

namespace Aprily.Backend.Features.Chat.UseCases.ListConversations;

public static class ListConversationsEndpoint
{
    public static void MapListConversationsEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/conversations", async (
            int? take,
            DateTime? before,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new ListConversationsQuery(take ?? 20, before);
            var result = await sender.Send(query, cancellationToken);

            return result.ToHttpResult();
        });
    }
}
