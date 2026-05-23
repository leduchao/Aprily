using Aprily.Application.Abstractions.Repositories;
using Aprily.Application.Abstractions.Services;
using Aprily.SharedKernel;

using MediatR;

namespace Aprily.Application.Users.Auth.SignOut;

public class SignOutCommandHandler(IUserRepository userRepository, IUserContext userContext) : IRequestHandler<SignOutCommand, Result>
{
    public async Task<Result> Handle(SignOutCommand request, CancellationToken cancellationToken)
    {
        var currentUser = await userRepository.GetUserByEntityId(userContext.UserEntityId);
        if (currentUser is null)
        {
            return Result.Failure(new Error("user.user_invalid", "User is invalid"));
        }

        await userRepository.RevokeAllRefreshTokens(currentUser.Id, cancellationToken);
        return Result.Success();
    }
}
