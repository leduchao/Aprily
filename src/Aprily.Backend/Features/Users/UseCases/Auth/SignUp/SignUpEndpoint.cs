using Aprily.Backend.Common.Results;

using MediatR;

namespace Aprily.Backend.Features.Users.UseCases.Auth.SignUp;

public static class SignUpEndpoint
{
    private record Request(string? FullName, string Username, string Email, string Password);

    public static void MapSignUpEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/sign-up", async (Request request, ISender sender, HttpContext httpContext) =>
        {
            var command = new SignUpCommand(request.FullName, request.Username, request.Email, request.Password);
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
