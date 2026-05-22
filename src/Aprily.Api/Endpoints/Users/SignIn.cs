using Aprily.Application.Users.SignIn;

using MediatR;

namespace Aprily.Api.Endpoints.Users;

public class SignIn : IEndpoint
{
    private record Request(string Email, string Password);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var users = app.MapUsers().AllowAnonymous();

        users.MapPost("/sign-in", async (
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new SignInCommand(request.Email, request.Password);
            var result = await sender.Send(command, ct);

            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
        });
    }
}
