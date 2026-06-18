using Aprily.Backend.Common.Extensions;

using MediatR;

namespace Aprily.Backend.Features.Chat.UseCases.OpenDirectConversation;

public static class OpenDirectConversationEndpoint
{
    private record Request(Guid RecipientUserId);

    public static void MapOpenDirectConversationEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/direct-conversations", async (
            Request request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new OpenDirectConversationCommand(request.RecipientUserId);
            var result = await sender.Send(command, cancellationToken);

            return result.ToHttpResult();
        });
    }
}
