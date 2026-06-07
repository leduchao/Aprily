using Aprily.Application.Users.Auth.SignIn;
using Aprily.SharedKernel;

using MediatR;

namespace Aprily.Api.Endpoints.Users.Auth;

internal class SignIn : IEndpoint
{
    private record Request(string Email, string Password);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapUsers()
            .MapPost(
                "/auth/sign-in",
                async (Request request, HttpContext httpContext, ISender sender, CancellationToken ct) =>
                {
                    var command = new SignInCommand(request.Email, request.Password);
                    var result = await sender.Send(command, ct);
                    if (result.IsFailure || result.Data is null)
                    {
                        return Results.BadRequest(result);
                    }

                    httpContext.Response.Cookies.Append(
                        "refreshToken",
                        result.Data.RefreshToken,
                        new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.None,
                            Expires = DateTimeOffset.UtcNow.AddDays(7)
                        });

                    return Results.Ok(Result<SignInResponse>.Success(
                        new SignInResponse(result.Data.AccessToken, string.Empty, result.Data.User)));
                })
            .WithTags(nameof(SignIn))
            .AllowAnonymous();
    }
}
