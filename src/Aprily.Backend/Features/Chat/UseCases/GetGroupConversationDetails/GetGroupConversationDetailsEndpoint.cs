using Aprily.Backend.Common.Extensions;
using MediatR;

namespace Aprily.Backend.Features.Chat.UseCases.GetGroupConversationDetails;

public static class GetGroupConversationDetailsEndpoint
{
    public static void MapGetGroupConversationDetailsEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/group-conversations/{conversationId:guid}", async (Guid conversationId, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetGroupConversationDetailsQuery(conversationId), cancellationToken);
            return result.ToHttpResult();
        });
    }
}
