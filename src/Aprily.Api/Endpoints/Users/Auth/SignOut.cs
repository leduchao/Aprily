using Aprily.Application.Users.Auth.SignOut;

using MediatR;

namespace Aprily.Api.Endpoints.Users.Auth;

public class SignOut : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapUsers()
            .MapPost(
                "/auth/sign-out",
                async (HttpContext httpContext, ISender sender, CancellationToken cancellation) =>
                {
                    var command = new SignOutCommand();
                    var result = await sender.Send(command, cancellation);

                    httpContext.Response.Cookies.Delete("refreshToken");

                    return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
                });
    }
}
