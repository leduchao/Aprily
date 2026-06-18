using Aprily.Backend.Common.Extensions;

using MediatR;

namespace Aprily.Backend.Features.Chat.UseCases.GetConversationMessages;

public static class GetConversationMessagesEndpoint
{
    public static void MapGetConversationMessagesEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/conversations/{conversationId:guid}/messages", async (
            Guid conversationId,
            int? take,
            DateTime? before,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetConversationMessagesQuery(conversationId, take ?? 30, before);
            var result = await sender.Send(query, cancellationToken);

            return result.ToHttpResult();
        });
    }
}
