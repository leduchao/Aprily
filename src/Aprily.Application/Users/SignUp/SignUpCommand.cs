using Aprily.Application.Abstractions.Cqrs;
using Aprily.Application.Users.GetUserProfile;

namespace Aprily.Application.Users.SignUp;

public record SignUpCommand(string Username, string Email, string Password, string? FullName = null) : ICommand<SignUpResponse>;

public record SignUpResponse(string AccessToken, UserProfileResponse User);
