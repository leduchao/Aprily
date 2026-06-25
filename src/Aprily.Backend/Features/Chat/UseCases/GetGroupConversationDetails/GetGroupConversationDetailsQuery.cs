using Aprily.Backend.Common.Results;
using Aprily.Backend.Database.Connection;
using Aprily.Backend.Features.Chat.Models;
using Aprily.Backend.Features.Users.Services;
using Dapper;
using MediatR;

namespace Aprily.Backend.Features.Chat.UseCases.GetGroupConversationDetails;

public record GetGroupConversationDetailsQuery(Guid ConversationId)
    : IRequest<Result<GroupConversationDetailsResponse>>;

public sealed class GetGroupConversationDetailsQueryHandler(IDbConnectionFactory connectionFactory, ICurrentUser currentUser)
    : IRequestHandler<GetGroupConversationDetailsQuery, Result<GroupConversationDetailsResponse>>
{
    public async Task<Result<GroupConversationDetailsResponse>> Handle(GetGroupConversationDetailsQuery request, CancellationToken cancellationToken)
    {
        using var connection = await connectionFactory.CreateConnection();
        var rows = (await connection.QueryAsync<Row>(new CommandDefinition(
            """
            SELECT c.entity_id AS ConversationId, gc.name, gc.avatar_url AS AvatarUrl,
                owner.entity_id AS OwnerId, owner.username AS OwnerUsername,
                owner.full_name AS OwnerFullName, owner.avatar_url AS OwnerAvatarUrl,
                current_member.role AS CurrentUserRole,
                member_user.entity_id AS MemberId, member_user.username AS MemberUsername,
                member_user.full_name AS MemberFullName, member_user.avatar_url AS MemberAvatarUrl,
                member.role AS MemberRole
            FROM conversations c
            INNER JOIN group_conversations gc ON gc.conversation_id = c.id AND gc.is_deleted = false
            INNER JOIN users owner ON owner.id = gc.created_by_user_id AND owner.is_deleted = false
            INNER JOIN conversation_members current_member ON current_member.conversation_id = c.id AND current_member.is_deleted = false
            INNER JOIN users viewer_user ON viewer_user.id = current_member.user_id AND viewer_user.entity_id = @CurrentUserId AND viewer_user.is_deleted = false
            INNER JOIN conversation_members member ON member.conversation_id = c.id AND member.is_deleted = false
            INNER JOIN users member_user ON member_user.id = member.user_id AND member_user.is_deleted = false
            WHERE c.entity_id = @ConversationId AND c.type = 'group' AND c.is_deleted = false
            ORDER BY CASE member.role WHEN 'owner' THEN 0 WHEN 'admin' THEN 1 ELSE 2 END,
                COALESCE(member_user.full_name, member_user.username);
            """,
            new { request.ConversationId, CurrentUserId = currentUser.UserEntityId },
            cancellationToken: cancellationToken))).ToList();

        if (rows.Count == 0)
            return Result<GroupConversationDetailsResponse>.Failure(new Error("chat.group_not_found", "Group conversation not found"));

        var first = rows[0];
        return Result<GroupConversationDetailsResponse>.Success(new(
            first.ConversationId, first.Name, first.AvatarUrl,
            new(first.OwnerId, first.OwnerUsername, first.OwnerFullName, first.OwnerAvatarUrl),
            first.CurrentUserRole,
            rows.Select(row => new GroupMemberResponse(row.MemberId, row.MemberUsername, row.MemberFullName, row.MemberAvatarUrl, row.MemberRole)).ToList()));
    }

    private sealed class Row
    {
        public Guid ConversationId { get; init; }
        public string Name { get; init; } = null!;
        public string? AvatarUrl { get; init; }
        public Guid OwnerId { get; init; }
        public string OwnerUsername { get; init; } = null!;
        public string? OwnerFullName { get; init; }
        public string? OwnerAvatarUrl { get; init; }
        public string CurrentUserRole { get; init; } = null!;
        public Guid MemberId { get; init; }
        public string MemberUsername { get; init; } = null!;
        public string? MemberFullName { get; init; }
        public string? MemberAvatarUrl { get; init; }
        public string MemberRole { get; init; } = null!;
    }
}
