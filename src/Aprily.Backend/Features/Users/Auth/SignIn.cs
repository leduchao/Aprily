using Aprily.Backend.Common.Extensions;
using Aprily.Backend.Common.Results;

using FluentValidation;

using MediatR;

namespace Aprily.Backend.Features.Users.Auth;

public static class SignIn
{
    public record Request(string Email, string Password);
    public record Response(string AccessToken, string Username, string? AvatarUrl);

    public record Command(string Email, string Password) : IRequest<Result<Response>>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(r => r.Email).NotEmpty().EmailAddress();
            RuleFor(r => r.Password).NotEmpty().MinimumLength(6).MaximumLength(16);
        }
    }

    public class Handler : IRequestHandler<Command, Result<Response>>
    {
        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var result = Result<Response>.Success(new Response("access_token", "username", "avatar_url"));
            await Task.CompletedTask;

            return result;
        }
    }

    public static void MapSignInEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/sign-in", async (Request request, ISender sender) =>
        {
            var command = new Command(request.Email, request.Password);
            var result = await sender.Send(command);

            return result.ToHttpResult();

        }).AllowAnonymous();
    }
}
