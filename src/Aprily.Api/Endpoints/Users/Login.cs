using Aprily.Application.Abstractions.Cqrs;
using Aprily.Application.Users;

using Microsoft.AspNetCore.Mvc;

namespace Aprily.Api.Endpoints.Users;

public class Login : IEndpoint
{
    private record Request(string Email, string Password);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/sign-in", async (
            [FromBody] Request request, 
            [FromServices] ICommandHandler<LoginCommand, LoginResponse> handler, 
            CancellationToken ct) =>
        {
            var command = new LoginCommand(request.Email, request.Password);
            var result = await handler.Handle(command, ct);

            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
        });
    }
}
