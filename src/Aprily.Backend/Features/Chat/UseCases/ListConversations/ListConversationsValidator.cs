using FluentValidation;

namespace Aprily.Backend.Features.Chat.UseCases.ListConversations;

public sealed class ListConversationsValidator : AbstractValidator<ListConversationsQuery>
{
    public ListConversationsValidator()
    {
        RuleFor(x => x.Take).InclusiveBetween(1, 50);
    }
}
