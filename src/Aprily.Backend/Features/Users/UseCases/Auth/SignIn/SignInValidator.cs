using FluentValidation;

namespace Aprily.Backend.Features.Users.UseCases.Auth.SignIn;

public sealed class SignInValidator : AbstractValidator<SignInCommand>
{
    public SignInValidator()
    {
        RuleFor(r => r.Email).NotEmpty().EmailAddress();
        RuleFor(r => r.Password).NotEmpty();
    }
}
