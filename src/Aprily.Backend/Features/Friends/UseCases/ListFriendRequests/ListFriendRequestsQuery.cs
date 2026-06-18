using Aprily.Backend.Common.Results;
using Aprily.Backend.Database.Connection;
using Aprily.Backend.Features.Friends.Models;
using Aprily.Backend.Features.Users.Services;

using Dapper;

using MediatR;

namespace Aprily.Backend.Features.Friends.UseCases.ListFriendRequests;

public sealed class ListFriendRequestsQuery(string direction, string status, int take)
    : IRequest<Result<IReadOnlyList<FriendRequestResponse>>>
{
    public string Direction { get; init; } = direction;
    public string Status { get; init; } = status;
    public int Take { get; init; } = take;

    public sealed class Handler(IDbConnectionFactory dbConnectionFactory, ICurrentUser currentUser)
        : IRequestHandler<ListFriendRequestsQuery, Result<IReadOnlyList<FriendRequestResponse>>>
    {
        private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;
        private readonly ICurrentUser _currentUser = currentUser;

        public async Task<Result<IReadOnlyList<FriendRequestResponse>>> Handle(
            ListFriendRequestsQuery request,
            CancellationToken cancellationToken)
        {
            using var conn = await _dbConnectionFactory.CreateConnection();

            var rows = await conn.QueryAsync<FriendRequestRow>(
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
                        fr.entity_id AS RequestId,
                        ru.entity_id AS RequesterId,
                        ru.username AS RequesterUsername,
                        ru.full_name AS RequesterFullName,
                        ru.email AS RequesterEmail,
                        ru.avatar_url AS RequesterAvatarUrl,
                        au.entity_id AS AddresseeId,
                        au.username AS AddresseeUsername,
                        au.full_name AS AddresseeFullName,
                        au.email AS AddresseeEmail,
                        au.avatar_url AS AddresseeAvatarUrl,
                        fr.status,
                        fr.created_at AS CreatedAt,
                        fr.responded_at AS RespondedAt
                    FROM me cu
                    INNER JOIN friend_requests fr
                        ON (
                            (@Direction = 'incoming' AND fr.addressee_user_id = cu.id)
                            OR (@Direction = 'outgoing' AND fr.requester_user_id = cu.id)
                        )
                        AND fr.is_deleted = false
                    INNER JOIN users ru ON ru.id = fr.requester_user_id
                    INNER JOIN users au ON au.id = fr.addressee_user_id
                    WHERE fr.status = @Status
                    ORDER BY fr.created_at DESC, fr.id DESC
                    LIMIT @Take;
                    """,
                    new
                    {
                        CurrentUserId = _currentUser.UserEntityId,
                        request.Direction,
                        request.Status,
                        request.Take
                    },
                    cancellationToken: cancellationToken));

            return Result<IReadOnlyList<FriendRequestResponse>>.Success(
                rows.Select(row => row.ToResponse()).ToList());
        }

        private sealed class FriendRequestRow
        {
            public Guid RequestId { get; init; }
            public Guid RequesterId { get; init; }
            public string RequesterUsername { get; init; } = null!;
            public string? RequesterFullName { get; init; }
            public string RequesterEmail { get; init; } = null!;
            public string? RequesterAvatarUrl { get; init; }
            public Guid AddresseeId { get; init; }
            public string AddresseeUsername { get; init; } = null!;
            public string? AddresseeFullName { get; init; }
            public string AddresseeEmail { get; init; } = null!;
            public string? AddresseeAvatarUrl { get; init; }
            public string Status { get; init; } = null!;
            public DateTime CreatedAt { get; init; }
            public DateTime? RespondedAt { get; init; }

            public FriendRequestResponse ToResponse()
            {
                return new FriendRequestResponse(
                    RequestId,
                    new FriendUserResponse(
                        RequesterId,
                        RequesterUsername,
                        RequesterFullName,
                        RequesterEmail,
                        RequesterAvatarUrl),
                    new FriendUserResponse(
                        AddresseeId,
                        AddresseeUsername,
                        AddresseeFullName,
                        AddresseeEmail,
                        AddresseeAvatarUrl),
                    Status,
                    CreatedAt,
                    RespondedAt);
            }
        }
    }
}
