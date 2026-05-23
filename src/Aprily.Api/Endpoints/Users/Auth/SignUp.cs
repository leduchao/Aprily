using Aprily.Application.Users.Auth.SignUp;

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
                async (Request request, ISender sender, CancellationToken ct) =>
                {
                    var command = new SignUpCommand(
                        request.Username,
                        request.Email,
                        request.Password,
                        request.FullName);

                    var result = await sender.Send(command, ct);
                    return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
                })
            .WithTags(nameof(SignUp))
            .AllowAnonymous();
    }
}
