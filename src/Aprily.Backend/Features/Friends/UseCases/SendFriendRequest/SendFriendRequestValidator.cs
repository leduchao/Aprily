using FluentValidation;

namespace Aprily.Backend.Features.Friends.UseCases.SendFriendRequest;

public sealed class SendFriendRequestValidator : AbstractValidator<SendFriendRequestCommand>
{
    public SendFriendRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => x.RecipientUserId is not null ^ !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Provide either recipientUserId or email");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}
