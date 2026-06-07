using Aprily.Application.Abstractions.Repositories;
using Aprily.SharedKernel;

using FluentValidation;

using MediatR;

namespace Aprily.Application.Chat.GetDirectConversations;

internal sealed class GetDirectConversationsQueryHandler(
    IChatRepository chatRepository,
    IValidator<GetDirectConversationsQuery> validator)
    : IRequestHandler<GetDirectConversationsQuery, Result<IReadOnlyList<DirectConversationResponse>>>
{
    public async Task<Result<IReadOnlyList<DirectConversationResponse>>> Handle(
        GetDirectConversationsQuery request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<IReadOnlyList<DirectConversationResponse>>.Failure(
                new Error("chat.invalid_conversations_request", validationResult.Errors[0].ErrorMessage));
        }

        try
        {
            IReadOnlyList<DirectConversationResponse> conversations =
                await chatRepository.GetDirectConversations(
                    request.CurrentUserId,
                    request.Take,
                    request.Before,
                    cancellationToken);

            return Result<IReadOnlyList<DirectConversationResponse>>.Success(conversations);
        }
        catch (KeyNotFoundException exception)
        {
            return Result<IReadOnlyList<DirectConversationResponse>>.Failure(
                new Error("chat.user_not_found", exception.Message));
        }
    }
}
