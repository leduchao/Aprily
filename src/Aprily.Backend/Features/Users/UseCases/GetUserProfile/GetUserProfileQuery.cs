using Aprily.Backend.Common.Results;
using Aprily.Backend.Database;
using Aprily.Backend.Features.Users.Models;
using Aprily.Backend.Features.Users.Services;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Aprily.Backend.Features.Users.UseCases.GetUserProfile;

public class GetUserProfileQuery(Guid userId) : IRequest<Result<UserBasicInfo>>
{
    public Guid UserId { get; init; } = userId;

    public class GetUserProfileQueryHandler(AppDbContext dbContext, IUserService userService) : IRequestHandler<GetUserProfileQuery, Result<UserBasicInfo>>
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly IUserService _userService = userService;

        public async Task<Result<UserBasicInfo>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(p => p.EntityId == request.UserId, cancellationToken);
            if (user is null)
            {
                return Result<UserBasicInfo>.Failure(new Error("users.user_notFound", "User not found"));
            }

            var result = new UserBasicInfo(
                user.EntityId,
                user.Username,
                user.FullName,
                user.Email,
                user.AvatarUrl,
                user.LastLoginAt,
                user.IsEmailVerified
            );

            return Result<UserBasicInfo>.Success(result);
        }
    }
}
