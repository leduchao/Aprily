using Aprily.Backend.Common.Extensions;

using MediatR;

namespace Aprily.Backend.Features.Users.UseCases.GetUserProfile;

public static class GetUserProfileEndpoint
{
    public static void MapGetUserProfileEndpoint(this RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet("/{userId}", async (Guid userId, ISender sender, CancellationToken cancellationToken) =>
        {
            var query = new GetUserProfileQuery(userId);
            var result = await sender.Send(query, cancellationToken);

            return result.ToHttpResult();
        }).AllowAnonymous();
    }
}
