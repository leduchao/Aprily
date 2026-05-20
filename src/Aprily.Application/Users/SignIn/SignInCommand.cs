using Aprily.Application.Abstractions.Cqrs;
using Aprily.Application.Users.GetUserProfile;
using Aprily.SharedKernel;

using MediatR;

namespace Aprily.Application.Users.SignIn;

public record SignInCommand(string Email, string Password) : IRequest<Result<SignInResponse>>; // ICommand<SignInResponse>;

public record SignInResponse(string AccessToken, UserProfileResponse User);
