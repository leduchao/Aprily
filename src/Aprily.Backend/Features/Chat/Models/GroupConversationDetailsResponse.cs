namespace Aprily.Backend.Features.Chat.Models;

public record GroupMemberResponse(
    Guid Id,
    string Username,
    string? FullName,
    string? AvatarUrl,
    string Role);

public record GroupConversationDetailsResponse(
    Guid ConversationId,
    string Name,
    string? AvatarUrl,
    ChatUserResponse Owner,
    string CurrentUserRole,
    IReadOnlyList<GroupMemberResponse> Members);
