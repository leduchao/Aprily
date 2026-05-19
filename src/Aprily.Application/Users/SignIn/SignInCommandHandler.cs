using Aprily.Application.Abstractions.Cqrs;
using Aprily.Application.Abstractions.Repositories;
using Aprily.Application.Abstractions.Services;
using Aprily.Application.Users.GetUserProfile;
using Aprily.SharedKernel;

using FluentValidation;

namespace Aprily.Application.Users.SignIn;

public class SignInCommandHandler(
    IUserRepository userRepository,
    ITokenProvider tokenProvider,
    IValidator<SignInCommand> validator) : ICommandHandler<SignInCommand, SignInResponse>
{
    public async Task<Result<SignInResponse>> Handle(SignInCommand command, CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(command, ct);
        if (!validationResult.IsValid)
        {
            var error = validationResult.Errors.Select(e => e.ErrorMessage).FirstOrDefault();
            return Result<SignInResponse>.Failure(
                new Error("users.sign_in_validation_failed", error ?? "Validation failed"));
        }

        var user = await userRepository.GetUserByEmail(command.Email);
        if (user == null || !PasswordHasher.Verify(command.Password, user.PasswordHash))
        {
            return Result<SignInResponse>.Failure(
                new Error("users.sign_in_failed", "Invalid email or password"));
        }

        user.LastLoginAt = DateTime.UtcNow;
        await userRepository.UpdateUser(user, ct);

        var accessToken = tokenProvider.GenerateToken(user);
        var userProfile = new UserProfileResponse(
            user.Username,
            user.FullName,
            user.Email,
            user.AvatarUrl,
            user.LastLoginAt,
            user.IsEmailVerified
        );

        var response = new SignInResponse(
            accessToken,
            userProfile
        );

        return Result<SignInResponse>.Success(response);
    }
}
