using Aprily.Application.Users.GetUserProfile;
using Aprily.SharedKernel;

using MediatR;

namespace Aprily.Application.Users.Auth.SignIn;

public record SignInCommand(string Email, string Password) : IRequest<Result<SignInResponse>>;

public record SignInResponse(string AccessToken, string RefreshToken, UserInfoDto User);
