using Aprily.SharedKernel;

using MediatR;

namespace Aprily.Application.Chat.GetDirectMessages;

public sealed record GetDirectMessagesQuery(
    Guid CurrentUserId,
    Guid OtherUserId,
    int Take = 50,
    DateTime? Before = null) : IRequest<Result<IReadOnlyList<ChatMessageResponse>>>;
