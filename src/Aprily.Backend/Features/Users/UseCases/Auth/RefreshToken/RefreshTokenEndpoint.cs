using Aprily.Backend.Common.Results;
using Aprily.Backend.Features.Users.UseCases.Auth;

using MediatR;

namespace Aprily.Backend.Features.Users.UseCases.Auth.RefreshToken;

public static class RefreshTokenEndpoint
{
    public static void MapRefreshTokenEndpoint(this RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost("/refresh-token", async (
            HttpContext httpContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var refreshToken = httpContext.Request.Cookies[AuthCookieOptions.RefreshTokenCookieName];

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(new RefreshTokenCommand(refreshToken), cancellationToken);
            if (result.IsFailure || result.Data is null)
            {
                httpContext.Response.Cookies.Delete(
                    AuthCookieOptions.RefreshTokenCookieName,
                    AuthCookieOptions.CreateDeleteRefreshTokenCookieOptions(httpContext.Request));

                return Results.Unauthorized();
            }

            httpContext.Response.Cookies.Append(
                AuthCookieOptions.RefreshTokenCookieName,
                result.Data.RefreshToken,
                AuthCookieOptions.CreateRefreshTokenCookieOptions(
                    httpContext.Request,
                    DateTimeOffset.UtcNow.AddDays(7)));

            return Results.Ok(Result<object>.Success(new
            {
                accessToken = result.Data.AccessToken,
                user = result.Data.User
            }));
        }).AllowAnonymous();
    }
}
