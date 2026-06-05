using Aprily.Application.Users.Auth.SignUp;
using Aprily.SharedKernel;

using MediatR;

namespace Aprily.Api.Endpoints.Users.Auth;

public class SignUp : IEndpoint
{
    private record Request(string Username, string Email, string Password, string? FullName);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapUsers()
            .MapPost(
                "/auth/sign-up",
                async (Request request, HttpContext httpContext, ISender sender, CancellationToken ct) =>
                {
                    var command = new SignUpCommand(
                        request.Username,
                        request.Email,
                        request.Password,
                        request.FullName);

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

                    return Results.Ok(Result<SignUpResponse>.Success(
                        new SignUpResponse(result.Data.AccessToken, string.Empty, result.Data.User)));
                })
            .WithTags(nameof(SignUp))
            .AllowAnonymous();
    }
}
