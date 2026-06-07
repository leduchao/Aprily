using Aprily.Application.Abstractions.Repositories;
using Aprily.SharedKernel;

using FluentValidation;

using MediatR;

namespace Aprily.Application.Chat.GetDirectMessages;

internal sealed class GetDirectMessagesQueryHandler(
    IChatRepository chatRepository,
    IValidator<GetDirectMessagesQuery> validator)
    : IRequestHandler<GetDirectMessagesQuery, Result<IReadOnlyList<ChatMessageResponse>>>
{
    public async Task<Result<IReadOnlyList<ChatMessageResponse>>> Handle(
        GetDirectMessagesQuery request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<IReadOnlyList<ChatMessageResponse>>.Failure(
                new Error("chat.invalid_history_request", validationResult.Errors[0].ErrorMessage));
        }

        try
        {
            IReadOnlyList<ChatMessageResponse> messages = await chatRepository.GetDirectMessages(
                request.CurrentUserId,
                request.OtherUserId,
                request.Take,
                request.Before,
                cancellationToken);

            return Result<IReadOnlyList<ChatMessageResponse>>.Success(messages);
        }
        catch (KeyNotFoundException exception)
        {
            return Result<IReadOnlyList<ChatMessageResponse>>.Failure(
                new Error("chat.user_not_found", exception.Message));
        }
    }
}
