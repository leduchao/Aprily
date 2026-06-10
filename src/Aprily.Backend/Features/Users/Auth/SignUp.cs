using Aprily.Backend.Common.Results;

using FluentValidation;

using MediatR;

namespace Aprily.Backend.Features.Users.Auth;

public static class SignUp
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

    public class Handler(IValidator<Command> validator) : IRequestHandler<Command, Result<Response>>
    {
        private readonly IValidator<Command> _validator = validator;

        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var result = Result<Response>.Success(new Response("access_token", "sign up", "avatar_url"));
            await Task.CompletedTask;

            return result;
        }
    }

    public static void MapSignUpEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/sign-up", async (Request request, ISender sender) =>
        {
            var command = new Command(request.Email, request.Password);
            var result = await sender.Send(command);

            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);

        }).AllowAnonymous();
    }
}