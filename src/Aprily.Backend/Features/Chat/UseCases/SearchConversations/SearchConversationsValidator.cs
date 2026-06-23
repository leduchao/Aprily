using FluentValidation;

namespace Aprily.Backend.Features.Chat.UseCases.SearchConversations;

public sealed class SearchConversationsValidator : AbstractValidator<SearchConversationsQuery>
{
    public SearchConversationsValidator()
    {
        RuleFor(candidate => candidate.Query)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(candidate => candidate.Take).InclusiveBetween(1, 50);
    }
}
