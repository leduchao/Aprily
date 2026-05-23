using Aprily.Application.Abstractions.Repositories;
using Aprily.Application.Abstractions.Services;
using Aprily.SharedKernel;

using FluentValidation;

using MediatR;

namespace Aprily.Application.Users.Auth.RefreshToken;

internal class RefreshTokenCommandHandler(
    IValidator<RefreshTokenCommand> validator,
    IUserRepository userRepository,
    ITokenProvider tokenProvider) : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var error = validationResult.Errors.Select(e => e.ErrorMessage).FirstOrDefault();
            return Result<RefreshTokenResponse>.Failure(
                new Error("users.refresh_token_validation_failed", error ?? "Validation failed"));
        }

        var storedToken = await userRepository.GetRefreshToken(request.Token, cancellationToken);
        if (storedToken is null || storedToken.IsRevoked || storedToken.ExpiryDate < DateTime.UtcNow)
        {
            return Result<RefreshTokenResponse>.Failure(
                new Error("users.refresh_token_validation_failed", "Stored token is invalid"));
        }

        if (storedToken.User is null)
        {
            return Result<RefreshTokenResponse>.Failure(
                new Error("users.user_invalid", "User is invalid"));
        }

        var newAccessToken = tokenProvider.GenerateAccessToken(storedToken.User);
        var newRefreshToken = tokenProvider.GenerateRefreshToken();

        storedToken.IsRevoked = true;
        storedToken.ReplacedByToken = newRefreshToken;

        var newStoredRefreshToken = new Domain.Entities.RefreshToken
        {
            UserId = storedToken.User.Id,
            Token = newRefreshToken,
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
        };

        await userRepository.AddRefreshToken(newStoredRefreshToken, cancellationToken);

        return Result<RefreshTokenResponse>.Success(new RefreshTokenResponse(newAccessToken, newRefreshToken));
    }
}
