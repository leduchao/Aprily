using Aprily.Backend.Common.Extensions;

using MediatR;

namespace Aprily.Backend.Features.Users.UseCases.Auth.SignOut;

public static class SingOutEnpoint
{
    public static void MapSignOutEndpoint(this RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost("/sign-out", async (HttpContext httpContext, ISender sender, CancellationToken cancellationToken) =>
        {
            var refreshToken = httpContext.Request.Cookies[AuthCookieOptions.RefreshTokenCookieName];
            Guid? currentUserId = null;

            if (httpContext.User.Identity?.IsAuthenticated == true)
            {
                currentUserId = httpContext.User.GetUserEntityId();
            }

            var result = await sender.Send(new SignOutCommand(refreshToken, currentUserId), cancellationToken);

            httpContext.Response.Cookies.Delete(
                AuthCookieOptions.RefreshTokenCookieName,
                AuthCookieOptions.CreateDeleteRefreshTokenCookieOptions(httpContext.Request));

            return result.ToHttpResult();
        }).AllowAnonymous();
    }
}
