using Aprily.Application.Abstractions.Cqrs;
using Aprily.Application.Abstractions.Repositories;
using Aprily.Application.Abstractions.Services;
using Aprily.Domain.Entities;
using Aprily.SharedKernel;
using FluentValidation;

namespace Aprily.Application.Users.SignUp;

public class SignUpCommandHandler(
    IUserRepository userRepository,
    ITokenProvider tokenProvider,
    IValidator<SignUpCommand> validator) : ICommandHandler<SignUpCommand, SignUpResponse>
{
    public async Task<Result<SignUpResponse>> Handle(SignUpCommand command, CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(command, ct);
        if (!validationResult.IsValid)
        {
            var error = validationResult.Errors.Select(e => e.ErrorMessage).FirstOrDefault();
            return Result<SignUpResponse>.Failure(
                new Error("users.sign_up_validation_failed", error ?? "Validation failed"));
        }

        var existingUser = await userRepository.GetUserByEmail(command.Email);
        if (existingUser != null)
        {
            return Result<SignUpResponse>.Failure(
                new Error("users.sign_up_failed", "A user with this email already exists"));
        }

        var user = new User
        {
            Username = command.Username,
            Email = command.Email,
            PasswordHash = PasswordHasher.Hash(command.Password),
            FullName = command.FullName,
            IsEmailVerified = false,
            LastLoginAt = DateTime.UtcNow
        };

        await userRepository.AddUser(user, ct);

        var accessToken = tokenProvider.GenerateToken(user);

        var response = new SignUpResponse(
            accessToken,
            user.Username,
            user.FullName,
            user.AvatarUrl,
            user.IsEmailVerified);

        return Result<SignUpResponse>.Success(response);
    }
}
