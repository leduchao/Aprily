using Aprily.Backend.Common.Extensions;

using MediatR;

namespace Aprily.Backend.Features.Users.UseCases.UpdateCurrentUserProfile;

public static class UpdateCurrentUserProfileEndpoint
{
    private sealed record Request(string? FullName);

    public static void MapUpdateCurrentUserProfileEndpoint(this RouteGroupBuilder group)
    {
        group.MapPatch("/me", async (
            Request request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new UpdateCurrentUserProfileCommand(request.FullName),
                cancellationToken);

            return result.ToHttpResult();
        });
    }
}
