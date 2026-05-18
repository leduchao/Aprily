using Aprily.Application.Abstractions.Cqrs;
using Aprily.Application.Users.SignUp;
using Microsoft.AspNetCore.Mvc;

namespace Aprily.Api.Endpoints.Users;

public class SignUp : IEndpoint
{
    private record Request(string Username, string Email, string Password, string? FullName);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost($"{BaseApiEndpoint.BasePath}/users/sign-up", async (
            [FromBody] Request request,
            [FromServices] ICommandHandler<SignUpCommand, SignUpResponse> handler,
            CancellationToken ct) =>
        {
            var command = new SignUpCommand(
                request.Username,
                request.Email,
                request.Password,
                request.FullName);

            var result = await handler.Handle(command, ct);
            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
        });
    }
}
