using FluentValidation;

namespace Aprily.Backend.Features.Users.UseCases.Auth.SignUp;

public sealed class Validator : AbstractValidator<SignUpCommand>
{
    public Validator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3).MaximumLength(50);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(16);
        RuleFor(x => x.FullName).MaximumLength(100);
    }
}
