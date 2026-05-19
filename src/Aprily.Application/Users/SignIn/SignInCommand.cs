using Aprily.Application.Abstractions.Cqrs;
using Aprily.Application.Users.GetUserProfile;

namespace Aprily.Application.Users.SignIn;

public record SignInCommand(string Email, string Password) : ICommand<SignInResponse>;

public record SignInResponse(string AccessToken, UserProfileResponse User);