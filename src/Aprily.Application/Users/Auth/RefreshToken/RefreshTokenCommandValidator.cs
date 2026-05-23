using FluentValidation;

namespace Aprily.Application.Users.Auth.RefreshToken;

internal class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(r => r.Token).NotEmpty();
    }
}
