using Aprily.Backend.Common.Extensions;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Aprily.Backend.Features.Chat.UseCases.SendDirectMessage;

public static class SendDirectMessageEndpoint
{
    private record Request(string Content, Guid? ReplyToMessageId);

    public static void MapSendDirectMessageEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/conversations/{conversationId:guid}/messages", async (
            Guid conversationId,
            Request request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new SendDirectMessageCommand(
                conversationId,
                request.Content,
                replyToMessageId: request.ReplyToMessageId);
            var result = await sender.Send(command, cancellationToken);

            return result.ToHttpResult();
        });

        group.MapPost("/conversations/{conversationId:guid}/image-messages", async (
            Guid conversationId,
            HttpRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var form = await request.ReadFormAsync(cancellationToken);
            var content = form["content"].ToString();
            var images = form.Files.GetFiles("images");
            Guid? replyToMessageId = Guid.TryParse(form["replyToMessageId"], out var parsedReplyToMessageId)
                ? parsedReplyToMessageId
                : null;

            var command = new SendDirectMessageCommand(conversationId, content, images, replyToMessageId);
            var result = await sender.Send(command, cancellationToken);

            return result.ToHttpResult();
        })
        .DisableAntiforgery()
        .WithMetadata(new RequestSizeLimitAttribute(42 * 1024 * 1024));
    }
}
