using Aprily.SharedKernel;

using MediatR;

namespace Aprily.Application.Chat.GetDirectConversations;

public sealed record GetDirectConversationsQuery(
    Guid CurrentUserId,
    int Take = 20,
    DateTime? Before = null) : IRequest<Result<IReadOnlyList<DirectConversationResponse>>>;
