using Aprily.Backend.Features.Friends.Models;

using Dapper;

namespace Aprily.Backend.Features.Friends.UseCases.RespondFriendRequest;

internal static class FriendRequestLoader
{
    public static async Task<FriendRequestResponse?> Load(
        System.Data.IDbConnection conn,
        Guid requestId,
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
                WHERE fr.entity_id = @RequestId
                AND fr.is_deleted = false;
                """,
                new { RequestId = requestId },
                cancellationToken: cancellationToken));

        return row?.ToResponse();
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
