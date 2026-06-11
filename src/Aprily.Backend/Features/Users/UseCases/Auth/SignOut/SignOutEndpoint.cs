using Aprily.Backend.Common.Extensions;

using MediatR;

namespace Aprily.Backend.Features.Users.UseCases.Auth.SignOut;

public static class SingOutEnpoint
{
    public static void MapSignOutEndpoint(this RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost("/sign-out", async (HttpContext httpContext, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new SignOutCommand(), cancellationToken);

            httpContext.Response.Cookies.Delete("refreshToken");

            return result.ToHttpResult();
        });
    }
}
