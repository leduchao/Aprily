using FluentValidation;

namespace Aprily.Application.Chat.GetDirectMessages;

internal sealed class GetDirectMessagesQueryValidator : AbstractValidator<GetDirectMessagesQuery>
{
    public GetDirectMessagesQueryValidator()
    {
        RuleFor(query => query.CurrentUserId).NotEmpty();
        RuleFor(query => query.OtherUserId)
            .NotEmpty()
            .NotEqual(query => query.CurrentUserId)
            .WithMessage("A direct message participant must be another user.");
        RuleFor(query => query.Take).InclusiveBetween(1, 100);
    }
}
