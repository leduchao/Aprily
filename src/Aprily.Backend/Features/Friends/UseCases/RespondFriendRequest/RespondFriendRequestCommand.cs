using Aprily.Backend.Common.Results;
using Aprily.Backend.Database.Connection;
using Aprily.Backend.Features.Friends.Models;
using Aprily.Backend.Features.Users.Services;

using Dapper;

using MediatR;

namespace Aprily.Backend.Features.Friends.UseCases.RespondFriendRequest;

public sealed class RespondFriendRequestCommand(Guid requestId, FriendRequestDecision decision)
    : IRequest<Result<FriendRequestResponse>>
{
    public Guid RequestId { get; init; } = requestId;
    public FriendRequestDecision Decision { get; init; } = decision;

    public sealed class Handler(IDbConnectionFactory dbConnectionFactory, ICurrentUser currentUser)
        : IRequestHandler<RespondFriendRequestCommand, Result<FriendRequestResponse>>
    {
        private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;
        private readonly ICurrentUser _currentUser = currentUser;

        public async Task<Result<FriendRequestResponse>> Handle(
            RespondFriendRequestCommand request,
            CancellationToken cancellationToken)
        {
            using var conn = await _dbConnectionFactory.CreateConnection();

            var requestRow = await conn.QueryFirstOrDefaultAsync<RequestInternalRow>(
                new CommandDefinition(
                    """
                    SELECT
                        fr.id AS InternalId,
                        fr.requester_user_id AS RequesterUserId,
                        fr.addressee_user_id AS AddresseeUserId
                    FROM friend_requests fr
                    INNER JOIN users addressee
                        ON addressee.id = fr.addressee_user_id
                        AND addressee.is_deleted = false
                    WHERE fr.entity_id = @RequestId
                    AND addressee.entity_id = @CurrentUserId
                    AND fr.status = 'pending'
                    AND fr.is_deleted = false
                    LIMIT 1;
                    """,
                    new
                    {
                        request.RequestId,
                        CurrentUserId = _currentUser.UserEntityId
                    },
                    cancellationToken: cancellationToken));

            if (requestRow is null)
            {
                return Result<FriendRequestResponse>.Failure(
                    new Error("friends.request_not_found", "Friend request not found"));
            }

            var status = request.Decision == FriendRequestDecision.Accept ? "accepted" : "declined";
            var now = DateTime.UtcNow;

            conn.Open();
            using var transaction = conn.BeginTransaction();

            await conn.ExecuteAsync(
                new CommandDefinition(
                    """
                    UPDATE friend_requests
                    SET status = @Status,
                        responded_at = @RespondedAt,
                        updated_at = @RespondedAt
                    WHERE id = @RequestInternalId;
                    """,
                    new
                    {
                        Status = status,
                        RespondedAt = now,
                        RequestInternalId = requestRow.InternalId
                    },
                    transaction,
                    cancellationToken: cancellationToken));

            if (request.Decision == FriendRequestDecision.Accept)
            {
                var userLowId = Math.Min(requestRow.RequesterUserId, requestRow.AddresseeUserId);
                var userHighId = Math.Max(requestRow.RequesterUserId, requestRow.AddresseeUserId);

                await conn.ExecuteAsync(
                    new CommandDefinition(
                        """
                        INSERT INTO friendships (
                            user_low_id,
                            user_high_id,
                            accepted_request_id,
                            entity_id
                        )
                        SELECT
                            @UserLowId,
                            @UserHighId,
                            @RequestInternalId,
                            @FriendshipId
                        WHERE NOT EXISTS (
                            SELECT 1
                            FROM friendships
                            WHERE user_low_id = @UserLowId
                            AND user_high_id = @UserHighId
                            AND is_deleted = false
                        );
                        """,
                        new
                        {
                            UserLowId = userLowId,
                            UserHighId = userHighId,
                            RequestInternalId = requestRow.InternalId,
                            FriendshipId = Guid.NewGuid()
                        },
                        transaction,
                        cancellationToken: cancellationToken));
            }

            transaction.Commit();

            var response = await FriendRequestLoader.Load(conn, request.RequestId, cancellationToken);

            return Result<FriendRequestResponse>.Success(response!);
        }

        private sealed class RequestInternalRow
        {
            public int InternalId { get; init; }
            public int RequesterUserId { get; init; }
            public int AddresseeUserId { get; init; }
        }
    }
}

public enum FriendRequestDecision
{
    Accept,
    Decline
}
