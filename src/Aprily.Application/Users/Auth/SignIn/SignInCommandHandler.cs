using Aprily.Application.Abstractions.Repositories;
using Aprily.Application.Abstractions.Services;
using Aprily.Application.Users.GetUserProfile;
using Aprily.SharedKernel;

using FluentValidation;

using MediatR;

namespace Aprily.Application.Users.Auth.SignIn;

public class SignInCommandHandler(
    IUserRepository userRepository,
    ITokenProvider tokenProvider,
    IValidator<SignInCommand> validator) : IRequestHandler<SignInCommand, Result<SignInResponse>>
{
    public async Task<Result<SignInResponse>> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var error = validationResult.Errors.Select(e => e.ErrorMessage).FirstOrDefault();
            return Result<SignInResponse>.Failure(
                new Error("users.sign_in_validation_failed", error ?? "Validation failed"));
        }

        var user = await userRepository.GetUserByEmail(request.Email);
        if (user == null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Result<SignInResponse>.Failure(
                new Error("users.sign_in_failed", "Invalid email or password"));
        }

        user.LastLoginAt = DateTime.UtcNow;
        await userRepository.UpdateUser(user, cancellationToken);

        var accessToken = tokenProvider.GenerateAccessToken(user);
        var userProfile = new UserInfoDto(
            user.EntityId,
            user.Username,
            user.FullName,
            user.Email,
            user.AvatarUrl,
            user.LastLoginAt,
            user.IsEmailVerified
        );

        var refreshToken = tokenProvider.GenerateRefreshToken();

        var newRefreshToken = new Domain.Entities.RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
        };

        await userRepository.AddRefreshToken(newRefreshToken, cancellationToken);

        var response = new SignInResponse(accessToken, refreshToken, userProfile);

        return Result<SignInResponse>.Success(response);
    }
}
