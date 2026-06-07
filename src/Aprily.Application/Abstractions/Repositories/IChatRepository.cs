using Aprily.Application.Chat;

namespace Aprily.Application.Abstractions.Repositories;

public interface IChatRepository
{
    Task<ChatMessageResponse> SendDirectMessage(
        Guid senderUserId,
        Guid recipientUserId,
        string content,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ChatMessageResponse>> GetDirectMessages(
        Guid currentUserId,
        Guid otherUserId,
        int take,
        DateTime? before,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<DirectConversationResponse>> GetDirectConversations(
        Guid currentUserId,
        int take,
        DateTime? before,
        CancellationToken cancellationToken);
}
