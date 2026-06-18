using FluentValidation;

namespace Aprily.Backend.Features.Friends.UseCases.ListFriendRequests;

public sealed class ListFriendRequestsValidator : AbstractValidator<ListFriendRequestsQuery>
{
    private static readonly string[] ValidDirections = ["incoming", "outgoing"];
    private static readonly string[] ValidStatuses = ["pending", "accepted", "declined", "canceled"];

    public ListFriendRequestsValidator()
    {
        RuleFor(x => x.Direction).Must(direction => ValidDirections.Contains(direction));
        RuleFor(x => x.Status).Must(status => ValidStatuses.Contains(status));
        RuleFor(x => x.Take).InclusiveBetween(1, 100);
    }
}
