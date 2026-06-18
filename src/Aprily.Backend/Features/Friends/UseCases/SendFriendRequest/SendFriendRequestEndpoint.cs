using Aprily.Backend.Common.Extensions;

using MediatR;

namespace Aprily.Backend.Features.Friends.UseCases.SendFriendRequest;

public static class SendFriendRequestEndpoint
{
    private record Request(Guid? RecipientUserId, string? Email);

    public static void MapSendFriendRequestEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/requests", async (
            Request request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new SendFriendRequestCommand(request.RecipientUserId, request.Email);
            var result = await sender.Send(command, cancellationToken);

            return result.ToHttpResult();
        });
    }
}
