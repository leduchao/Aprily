using Aprily.Application.Abstractions.Repositories;
using Aprily.SharedKernel;

using MediatR;

namespace Aprily.Application.Users.GetUserProfile;

public class GetUserProfileQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUserProfileQuery, Result<UserInfoDto>>
{
    public async Task<Result<UserInfoDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByEmail(request.Email);
        return user == null
            ? Result<UserInfoDto>.Failure(new Error("users.not_found", "User not found"))
            : Result<UserInfoDto>.Success(
                new UserInfoDto(
                    user.EntityId,
                    user.Username,
                    user.FullName,
                    user.Email,
                    user.AvatarUrl,
                    user.LastLoginAt,
                    user.IsEmailVerified
                ));
    }
}