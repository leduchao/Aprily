using FluentValidation;

namespace Aprily.Backend.Features.Friends.UseCases.ListFriends;

public sealed class ListFriendsValidator : AbstractValidator<ListFriendsQuery>
{
    public ListFriendsValidator()
    {
        RuleFor(x => x.Take).InclusiveBetween(1, 100);
    }
}
