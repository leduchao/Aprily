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
                    u.entity_id AS Id,
                    u.username AS Username,
                    u.full_name AS FullName,
                    u.email AS Email,
                    u.avatar_url AS AvatarUrl,
                    u.last_sign_in_at AS LastSignInAt,
                    u.is_email_verified AS IsEmailVerified
                FROM users AS u
                WHERE u.entity_id = @UserId
                AND u.is_deleted = false;
            """;

            var user = await conn.QueryFirstOrDefaultAsync<UserBasicInfo>(sql, new { request.UserId });

            return user is null
                ? Result<UserBasicInfo>.Failure(new Error("users.user_notFound", "User not found"))
                : Result<UserBasicInfo>.Success(user);
        }
    }
}
