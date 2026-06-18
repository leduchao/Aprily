using Aprily.Backend.Common.Results;
using Aprily.Backend.Features.Users.UseCases.Auth;

using MediatR;

namespace Aprily.Backend.Features.Users.UseCases.Auth.SignIn;

public static class Enpoint
{
    private record Request(string Email, string Password);

    public static void MapSignInEndpoint(this RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost("/sign-in", async (Request request, ISender sender, HttpContext httpContext) =>
        {
            var command = new SignInCommand(request.Email, request.Password);
            var result = await sender.Send(command);
            if (result.IsFailure || result.Data is null)
            {
                return Results.BadRequest(result);
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
