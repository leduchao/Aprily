using Aprily.Application.Users.Auth.RefreshToken;
using Aprily.SharedKernel;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Aprily.Api.Endpoints.Users.Auth;

public class RefreshToken : IEndpoint
{
    private record Request(string Token);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapUsers()
            .MapPost(
                "/auth/refresh-token",
                async (HttpContext httpContext, ISender sender, CancellationToken cancellation) =>
                {
                    var refreshToken = httpContext.Request.Cookies["refreshToken"];
                    if (string.IsNullOrWhiteSpace(refreshToken))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new RefreshTokenCommand(refreshToken);
                    var result = await sender.Send(command, cancellation);
                    if (result.IsFailure || result.Data is null)
                    {
                        return Results.Unauthorized();
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

                    return Results.Ok(Result<RefreshTokenResponse>.Success(
                        new RefreshTokenResponse(result.Data.AccessToken, string.Empty)));
                })
            .AllowAnonymous();
    }
}
