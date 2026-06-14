using Aprily.Backend.Common.Results;
using Aprily.Backend.Database;
using Aprily.Backend.Database.Connection;
using Aprily.Backend.Entities;
using Aprily.Backend.Features.Users.Models;

using Dapper;

using MediatR;

namespace Aprily.Backend.Features.Users.UseCases.GetUserProfile;

public class GetUserProfileQuery(Guid userId) : IRequest<Result<UserBasicInfo>>
{
    public Guid UserId { get; init; } = userId;

    public class GetUserProfileQueryHandler(IDbConnectionFactory dbConnectionFactory) : IRequestHandler<GetUserProfileQuery, Result<UserBasicInfo>>
    {
        private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;

        public async Task<Result<UserBasicInfo>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            using var conn = await _dbConnectionFactory.CreateConnection();

            var sql = $"""
                SELECT
                    u.EntityId AS Id,
                    u.Username,
                    u.FullName,
                    u.Email,
                    u.AvatarUrl,
                    u.LastLoginAt,
                    u.IsEmailVerified
                FROM {nameof(AppDbContext.Users)} AS u
                WHERE u.{nameof(User.EntityId)} = @UserId AND u.IsDeleted = 0;
            """;

            var user = await conn.QueryFirstOrDefaultAsync<UserBasicInfo>(sql, new { request.UserId });

            return user is null
                ? Result<UserBasicInfo>.Failure(new Error("users.user_notFound", "User not found"))
                : Result<UserBasicInfo>.Success(user);
        }
    }
}
