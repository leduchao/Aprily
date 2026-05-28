using Aprily.Application.Abstractions.Repositories;
using Aprily.Application.Abstractions.Services;
using Aprily.Application.Users.GetUserProfile;
using Aprily.Domain.Entities;
using Aprily.SharedKernel;

using FluentValidation;

using MediatR;

namespace Aprily.Application.Users.Auth.SignUp;

public class SignUpCommandHandler(
    IUserRepository userRepository,
    ITokenProvider tokenProvider,
    IValidator<SignUpCommand> validator) : IRequestHandler<SignUpCommand, Result<SignUpResponse>>
{
    public async Task<Result<SignUpResponse>> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var error = validationResult.Errors.Select(e => e.ErrorMessage).FirstOrDefault();
            return Result<SignUpResponse>.Failure(
                new Error("users.sign_up_validation_failed", error ?? "Validation failed"));
        }

        var existingUser = await userRepository.GetUserByEmail(request.Email);
        if (existingUser != null)
        {
            return Result<SignUpResponse>.Failure(
                new Error("users.sign_up_failed", "A user with this email already exists"));
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = PasswordHasher.Hash(request.Password),
            FullName = request.FullName,
            IsEmailVerified = false,
            LastLoginAt = DateTime.UtcNow
        };

        await userRepository.AddUser(user, cancellationToken);

        var accessToken = tokenProvider.GenerateAccessToken(user);
        var userProfile = new UserInfoDto(
            user.EntityId,
            user.Username,
            user.FullName,
            user.Email,
            user.AvatarUrl,
            user.LastLoginAt,
            user.IsEmailVerified);

        var refreshToken = tokenProvider.GenerateRefreshToken();

        var newRefreshToken = new Domain.Entities.RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
        };

        await userRepository.AddRefreshToken(newRefreshToken, cancellationToken);

        var response = new SignUpResponse(accessToken, refreshToken, userProfile);

        return Result<SignUpResponse>.Success(response);
    }
}
