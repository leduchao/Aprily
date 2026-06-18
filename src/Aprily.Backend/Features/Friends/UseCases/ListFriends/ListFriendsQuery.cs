using Aprily.Backend.Common.Results;
using Aprily.Backend.Database.Connection;
using Aprily.Backend.Features.Friends.Models;
using Aprily.Backend.Features.Users.Services;

using Dapper;

using MediatR;

namespace Aprily.Backend.Features.Friends.UseCases.ListFriends;

public sealed class ListFriendsQuery(int take, DateTime? before)
    : IRequest<Result<IReadOnlyList<FriendResponse>>>
{
    public int Take { get; init; } = take;
    public DateTime? Before { get; init; } = before;

    public sealed class Handler(IDbConnectionFactory dbConnectionFactory, ICurrentUser currentUser)
        : IRequestHandler<ListFriendsQuery, Result<IReadOnlyList<FriendResponse>>>
    {
        private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;
        private readonly ICurrentUser _currentUser = currentUser;

        public async Task<Result<IReadOnlyList<FriendResponse>>> Handle(
            ListFriendsQuery request,
            CancellationToken cancellationToken)
        {
            using var conn = await _dbConnectionFactory.CreateConnection();

            var rows = await conn.QueryAsync<FriendRow>(
                new CommandDefinition(
                    """
                    WITH me AS (
                        SELECT id
                        FROM users
                        WHERE entity_id = @CurrentUserId
                        AND is_deleted = false
                        LIMIT 1
                    )
                    SELECT
                        f.entity_id AS FriendshipId,
                        friend.entity_id AS UserId,
                        friend.username AS Username,
                        friend.full_name AS FullName,
                        friend.email AS Email,
                        friend.avatar_url AS AvatarUrl,
                        f.created_at AS CreatedAt
                    FROM me cu
                    INNER JOIN friendships f
                        ON (f.user_low_id = cu.id OR f.user_high_id = cu.id)
                        AND f.is_deleted = false
                    INNER JOIN users friend
                        ON friend.id = CASE
                            WHEN f.user_low_id = cu.id THEN f.user_high_id
                            ELSE f.user_low_id
                        END
                        AND friend.is_deleted = false
                    WHERE (@Before::timestamptz IS NULL OR f.created_at < @Before::timestamptz)
                    ORDER BY f.created_at DESC, f.id DESC
                    LIMIT @Take;
                    """,
                    new
                    {
                        CurrentUserId = _currentUser.UserEntityId,
                        request.Before,
                        request.Take
                    },
                    cancellationToken: cancellationToken));

            var friends = rows
                .Select(row => new FriendResponse(
                    row.FriendshipId,
                    new FriendUserResponse(
                        row.UserId,
                        row.Username,
                        row.FullName,
                        row.Email,
                        row.AvatarUrl),
                    row.CreatedAt))
                .ToList();

            return Result<IReadOnlyList<FriendResponse>>.Success(friends);
        }

        private sealed class FriendRow
        {
            public Guid FriendshipId { get; init; }
            public Guid UserId { get; init; }
            public string Username { get; init; } = null!;
            public string? FullName { get; init; }
            public string Email { get; init; } = null!;
            public string? AvatarUrl { get; init; }
            public DateTime CreatedAt { get; init; }
        }
    }
}
