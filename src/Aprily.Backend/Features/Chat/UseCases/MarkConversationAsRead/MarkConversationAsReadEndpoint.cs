using Aprily.Backend.Common.Extensions;

using MediatR;

namespace Aprily.Backend.Features.Chat.UseCases.MarkConversationAsRead;

public static class MarkConversationAsReadEndpoint
{
    private record Request(Guid MessageId);

    public static void MapMarkConversationAsReadEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/conversations/{conversationId:guid}/read", async (
            Guid conversationId,
            Request request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new MarkConversationAsReadCommand(conversationId, request.MessageId);
            var result = await sender.Send(command, cancellationToken);

            return result.ToHttpResult();
        });
    }
}
