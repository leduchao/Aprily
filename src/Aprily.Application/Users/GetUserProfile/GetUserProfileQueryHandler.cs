using Aprily.Application.Abstractions.Repositories;
using Aprily.SharedKernel;

using MediatR;

namespace Aprily.Application.Users.GetUserProfile;

public class GetUserProfileQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUserProfileQuery, Result<UserProfileResponse>>
{
    public async Task<Result<UserProfileResponse>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByEmail(request.Email);
        return user == null
            ? Result<UserProfileResponse>.Failure(new Error("users.not_found", "User not found"))
            : Result<UserProfileResponse>.Success(
                new UserProfileResponse(
                    user.Username,
                    user.FullName,
                    user.Email,
                    user.AvatarUrl,
                    user.LastLoginAt,
                    user.IsEmailVerified
                ));
    }
}