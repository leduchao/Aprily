using Aprily.Application.Abstractions.Cqrs;

namespace Aprily.Application.Users.SignUp;

public record SignUpCommand(string Username, string Email, string Password, string? FullName = null) : ICommand<SignUpResponse>;

public record SignUpResponse(string AccessToken, string Username, string? FullName, string? AvatarUrl, bool IsEmailVerified);
