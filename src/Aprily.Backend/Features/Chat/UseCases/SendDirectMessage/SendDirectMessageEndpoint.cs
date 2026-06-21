using Aprily.Backend.Common.Extensions;

using MediatR;

using Microsoft.AspNetCore.Mvc;

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

        group.MapPost("/direct-image-messages", async (
            HttpRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var form = await request.ReadFormAsync(cancellationToken);
            var recipientUserId = Guid.TryParse(form["recipientUserId"], out var parsedRecipientUserId)
                ? parsedRecipientUserId
                : Guid.Empty;
            var content = form["content"].ToString();
            var images = form.Files.GetFiles("images");

            var command = new SendDirectMessageCommand(recipientUserId, content, images);
            var result = await sender.Send(command, cancellationToken);

            return result.ToHttpResult();
        })
        .DisableAntiforgery()
        .WithMetadata(new RequestSizeLimitAttribute(42 * 1024 * 1024));
    }
}
