using Aprily.Backend.Common.Extensions;

using MediatR;

namespace Aprily.Backend.Features.Users.UseCases.UploadCurrentUserAvatar;

public static class UploadCurrentUserAvatarEndpoint
{
    public static void MapUploadCurrentUserAvatarEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/me/avatar", async (
            IFormFile avatar,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new UploadCurrentUserAvatarCommand(avatar),
                cancellationToken);

            return result.ToHttpResult();
        }).DisableAntiforgery();
    }
}
