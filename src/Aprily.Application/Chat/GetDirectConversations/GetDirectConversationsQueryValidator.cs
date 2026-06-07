using FluentValidation;

namespace Aprily.Application.Chat.GetDirectConversations;

internal sealed class GetDirectConversationsQueryValidator : AbstractValidator<GetDirectConversationsQuery>
{
    public GetDirectConversationsQueryValidator()
    {
        RuleFor(query => query.CurrentUserId).NotEmpty();
        RuleFor(query => query.Take).InclusiveBetween(1, 100);
    }
}
