using Aprily.SharedKernel;

using MediatR;

namespace Aprily.Application.Chat.SendDirectMessage;

public sealed record SendDirectMessageCommand(
    Guid SenderUserId,
    Guid RecipientUserId,
    string Content) : IRequest<Result<ChatMessageResponse>>;
