using Aprily.Application.Users.GetUserProfile;
using Aprily.SharedKernel;

using MediatR;

namespace Aprily.Application.Users.Auth.SignUp;

public record SignUpCommand(string Username, string Email, string Password, string? FullName = null) : IRequest<Result<SignUpResponse>>;

public record SignUpResponse(string AccessToken, string RefreshToken, UserInfoDto User);
