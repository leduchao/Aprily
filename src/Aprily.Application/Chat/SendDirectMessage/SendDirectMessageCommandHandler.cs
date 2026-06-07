using Aprily.Application.Abstractions.Repositories;
using Aprily.SharedKernel;

using FluentValidation;

using MediatR;

namespace Aprily.Application.Chat.SendDirectMessage;

internal sealed class SendDirectMessageCommandHandler(
    IChatRepository chatRepository,
    IValidator<SendDirectMessageCommand> validator)
    : IRequestHandler<SendDirectMessageCommand, Result<ChatMessageResponse>>
{
    public async Task<Result<ChatMessageResponse>> Handle(
        SendDirectMessageCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<ChatMessageResponse>.Failure(
                new Error("chat.invalid_message", validationResult.Errors[0].ErrorMessage));
        }

        try
        {
            ChatMessageResponse message = await chatRepository.SendDirectMessage(
                request.SenderUserId,
                request.RecipientUserId,
                request.Content.Trim(),
                cancellationToken);

            return Result<ChatMessageResponse>.Success(message);
        }
        catch (KeyNotFoundException exception)
        {
            return Result<ChatMessageResponse>.Failure(
                new Error("chat.user_not_found", exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return Result<ChatMessageResponse>.Failure(
                new Error("chat.sender_not_found", exception.Message));
        }
    }
}
