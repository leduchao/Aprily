namespace Aprily.Backend.Features.Chat.Models;

public record MessageReactionsUpdatedResponse(
    Guid ConversationId,
    Guid MessageId,
    Guid ActorUserId,
    IReadOnlyList<MessageReactionSummaryResponse> Reactions);
