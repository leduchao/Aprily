using Aprily.Backend.Common.Extensions;

using MediatR;

namespace Aprily.Backend.Features.Chat.UseCases.SetMessageReaction;

public static class SetMessageReactionEndpoint
{
    private sealed record Request(string? Type);

    public static void MapSetMessageReactionEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("/messages/{messageId:guid}/reaction", async (
            Guid messageId,
            Request request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new SetMessageReactionCommand(messageId, request.Type),
                cancellationToken);

            return result.ToHttpResult();
        });
    }
}
