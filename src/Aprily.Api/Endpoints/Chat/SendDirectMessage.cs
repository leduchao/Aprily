using Aprily.Api.Hubs;
using Aprily.Application.Abstractions.Services;
using Aprily.Application.Chat;
using Aprily.Application.Chat.SendDirectMessage;
using Aprily.SharedKernel;

using MediatR;

using Microsoft.AspNetCore.SignalR;

namespace Aprily.Api.Endpoints.Chat;

internal sealed class SendDirectMessage : IEndpoint
{
    private sealed record Request(Guid RecipientUserId, string Content);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapChat()
            .MapPost(
                "/direct-messages",
                async (
                    Request request,
                    IUserContext userContext,
                    ISender sender,
                    IHubContext<ChatHub, IChatClient> hubContext,
                    CancellationToken cancellationToken) =>
                {
                    var command = new SendDirectMessageCommand(
                        userContext.UserEntityId,
                        request.RecipientUserId,
                        request.Content);
                    Result<ChatMessageResponse> result = await sender.Send(command, cancellationToken);

                    if (result.IsFailure || result.Data is null)
                    {
                        return result.Error?.Code == "chat.user_not_found"
                            ? Results.NotFound(result)
                            : Results.BadRequest(result);
                    }

                    await hubContext.Clients
                        .Users(
                            userContext.UserEntityId.ToString(),
                            request.RecipientUserId.ToString())
                        .ReceiveDirectMessage(result.Data);

                    return Results.Ok(result);
                })
            .WithName(nameof(SendDirectMessage));
    }
}
