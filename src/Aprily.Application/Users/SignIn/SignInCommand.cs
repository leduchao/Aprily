using Aprily.Application.Abstractions.Cqrs;

namespace Aprily.Application.Users.SignIn;

public record SignInCommand(string Email, string Password) : ICommand<SignInResponse>;

public record SignInResponse(string AccessToken, string Username, string? FullName, string? AvatarUrl, bool IsEmailVerified);