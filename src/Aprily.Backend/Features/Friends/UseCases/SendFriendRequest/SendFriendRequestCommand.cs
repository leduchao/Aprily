using Aprily.Backend.Common.Results;
using Aprily.Backend.Database.Connection;
using Aprily.Backend.Features.Chat.Hubs;
using Aprily.Backend.Features.Friends.Models;
using Aprily.Backend.Features.Users.Services;

using Dapper;

using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Aprily.Backend.Features.Friends.UseCases.SendFriendRequest;

public sealed class SendFriendRequestCommand(Guid? recipientUserId, string? email)
    : IRequest<Result<FriendRequestResponse>>
{
    public Guid? RecipientUserId { get; init; } = recipientUserId;
    public string? Email { get; init; } = email;

    public sealed class Handler(
        IDbConnectionFactory dbConnectionFactory,
        ICurrentUser currentUser,
        IHubContext<ChatHub> chatHub)
        : IRequestHandler<SendFriendRequestCommand, Result<FriendRequestResponse>>
    {
        private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;
        private readonly ICurrentUser _currentUser = currentUser;
        private readonly IHubContext<ChatHub> _chatHub = chatHub;

        public async Task<Result<FriendRequestResponse>> Handle(
            SendFriendRequestCommand request,
            CancellationToken cancellationToken)
        {
            using var conn = await _dbConnectionFactory.CreateConnection();

            var requester = await conn.QueryFirstOrDefaultAsync<UserRow>(
                new CommandDefinition(
                    UserByEntityIdSql,
                    new { UserId = _currentUser.UserEntityId },
                    cancellationToken: cancellationToken));

            if (requester is null)
            {
                return Result<FriendRequestResponse>.Failure(
                    new Error("users.user_notFound", "User not found"));
            }

            var addressee = request.RecipientUserId is not null
                ? await conn.QueryFirstOrDefaultAsync<UserRow>(
                    new CommandDefinition(
                        UserByEntityIdSql,
                        new { UserId = request.RecipientUserId.Value },
                        cancellationToken: cancellationToken))
                : await conn.QueryFirstOrDefaultAsync<UserRow>(
                    new CommandDefinition(
                        UserByEmailSql,
                        new { Email = request.Email!.Trim() },
                        cancellationToken: cancellationToken));

            if (addressee is null)
            {
                return Result<FriendRequestResponse>.Failure(
                    new Error("friends.user_not_found", "User not found"));
            }

            if (requester.InternalId == addressee.InternalId)
            {
                return Result<FriendRequestResponse>.Failure(
                    new Error("friends.cannot_add_self", "Cannot send a friend request to yourself"));
            }

            var userLowId = Math.Min(requester.InternalId, addressee.InternalId);
            var userHighId = Math.Max(requester.InternalId, addressee.InternalId);

            var alreadyFriends = await conn.ExecuteScalarAsync<bool>(
                new CommandDefinition(
                    """
                    SELECT EXISTS (
                        SELECT 1
                        FROM friendships
                        WHERE user_low_id = @UserLowId
                        AND user_high_id = @UserHighId
                        AND is_deleted = false
                    );
                    """,
                    new { UserLowId = userLowId, UserHighId = userHighId },
                    cancellationToken: cancellationToken));

            if (alreadyFriends)
            {
                return Result<FriendRequestResponse>.Failure(
                    new Error("friends.already_friends", "Users are already friends"));
            }

            var pendingRequestId = await conn.QueryFirstOrDefaultAsync<Guid?>(
                new CommandDefinition(
                    """
                    SELECT entity_id
                    FROM friend_requests
                    WHERE user_low_id = @UserLowId
                    AND user_high_id = @UserHighId
                    AND status = 'pending'
                    AND is_deleted = false
                    LIMIT 1;
                    """,
                    new { UserLowId = userLowId, UserHighId = userHighId },
                    cancellationToken: cancellationToken));

            if (pendingRequestId is not null)
            {
                return Result<FriendRequestResponse>.Failure(
                    new Error("friends.request_already_pending", "A friend request is already pending"));
            }

            var friendRequestId = Guid.NewGuid();

            await conn.ExecuteAsync(
                new CommandDefinition(
                    """
                    INSERT INTO friend_requests (
                        requester_user_id,
                        addressee_user_id,
                        status,
                        entity_id
                    )
                    VALUES (
                        @RequesterUserId,
                        @AddresseeUserId,
                        'pending',
                        @FriendRequestId
                    );
                    """,
                    new
                    {
                        RequesterUserId = requester.InternalId,
                        AddresseeUserId = addressee.InternalId,
                        FriendRequestId = friendRequestId
                    },
                    cancellationToken: cancellationToken));

            var response = await LoadFriendRequest(conn, friendRequestId, cancellationToken);

            await _chatHub.Clients
                .Groups(
                    ChatHub.UserGroup(requester.Id),
                    ChatHub.UserGroup(addressee.Id))
                .SendAsync(
                    ChatHub.FriendRequestsUpdatedEvent,
                    cancellationToken);

            return Result<FriendRequestResponse>.Success(response!);
        }

        private static async Task<FriendRequestResponse?> LoadFriendRequest(
            System.Data.IDbConnection conn,
            Guid friendRequestId,
            CancellationToken cancellationToken)
        {
            var row = await conn.QueryFirstOrDefaultAsync<FriendRequestRow>(
                new CommandDefinition(
                    """
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
                    FROM friend_requests fr
                    INNER JOIN users ru ON ru.id = fr.requester_user_id
                    INNER JOIN users au ON au.id = fr.addressee_user_id
                    WHERE fr.entity_id = @FriendRequestId
                    AND fr.is_deleted = false;
                    """,
                    new { FriendRequestId = friendRequestId },
                    cancellationToken: cancellationToken));

            return row?.ToResponse();
        }

        private const string UserByEntityIdSql = """
            SELECT
                id AS InternalId,
                entity_id AS Id,
                username AS Username,
                full_name AS FullName,
                email AS Email,
                avatar_url AS AvatarUrl
            FROM users
            WHERE entity_id = @UserId
            AND is_deleted = false
            LIMIT 1;
            """;

        private const string UserByEmailSql = """
            SELECT
                id AS InternalId,
                entity_id AS Id,
                username AS Username,
                full_name AS FullName,
                email AS Email,
                avatar_url AS AvatarUrl
            FROM users
            WHERE lower(email) = lower(@Email)
            AND is_deleted = false
            LIMIT 1;
            """;

        private sealed class UserRow
        {
            public int InternalId { get; init; }
            public Guid Id { get; init; }
            public string Username { get; init; } = null!;
            public string? FullName { get; init; }
            public string Email { get; init; } = null!;
            public string? AvatarUrl { get; init; }
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
