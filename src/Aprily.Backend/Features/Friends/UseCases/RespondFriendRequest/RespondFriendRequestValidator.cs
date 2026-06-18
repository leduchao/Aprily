using FluentValidation;

namespace Aprily.Backend.Features.Friends.UseCases.RespondFriendRequest;

public sealed class RespondFriendRequestValidator : AbstractValidator<RespondFriendRequestCommand>
{
    public RespondFriendRequestValidator()
    {
        RuleFor(x => x.RequestId).NotEmpty();
        RuleFor(x => x.Decision).IsInEnum();
    }
}
