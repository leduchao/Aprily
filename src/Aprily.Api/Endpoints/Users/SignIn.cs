using Aprily.Application.Abstractions.Cqrs;
using Aprily.Application.Users.SignIn;

using Microsoft.AspNetCore.Mvc;

namespace Aprily.Api.Endpoints.Users;

public class SignIn : IEndpoint
{
    private record Request(string Email, string Password);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost($"{BaseApiEndpoint.BasePath}/users/sign-in", async (
            [FromBody] Request request,
            [FromServices] ICommandHandler<SignInCommand, SignInResponse> handler,
            CancellationToken ct) =>
        {
            var command = new SignInCommand(request.Email, request.Password);
            var result = await handler.Handle(command, ct);

            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
        });
    }
}
