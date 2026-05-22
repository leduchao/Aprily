using Aprily.Application.Abstractions.Repositories;
using Aprily.Application.Abstractions.Services;
using Aprily.Application.Users.GetUserProfile;
using Aprily.SharedKernel;

using FluentValidation;

using MediatR;

namespace Aprily.Application.Users.SignIn;

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

        var accessToken = tokenProvider.GenerateToken(user);
        var userProfile = new UserProfileResponse(
            user.EntityId,
            user.Username,
            user.FullName,
            user.Email,
            user.AvatarUrl,
            user.LastLoginAt,
            user.IsEmailVerified
        );

        var response = new SignInResponse(accessToken, userProfile);

        return Result<SignInResponse>.Success(response);
    }
}
