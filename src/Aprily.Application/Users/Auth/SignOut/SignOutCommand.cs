using Aprily.SharedKernel;

using MediatR;

namespace Aprily.Application.Users.Auth.SignOut;

public record SignOutCommand() : IRequest<Result>;
