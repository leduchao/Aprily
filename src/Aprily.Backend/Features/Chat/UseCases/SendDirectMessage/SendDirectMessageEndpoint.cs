using Aprily.Backend.Common.Extensions;

using MediatR;

namespace Aprily.Backend.Features.Chat.UseCases.SendDirectMessage;

public static class SendDirectMessageEndpoint
{
    private record Request(Guid RecipientUserId, string Content);

    public static void MapSendDirectMessageEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/direct-messages", async (
            Request request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new SendDirectMessageCommand(request.RecipientUserId, request.Content);
            var result = await sender.Send(command, cancellationToken);

            return result.ToHttpResult();
        });
    }
}
