using FluentValidation;

namespace Aprily.Backend.Features.Chat.UseCases.SendDirectMessage;

public sealed class SendDirectMessageValidator : AbstractValidator<SendDirectMessageCommand>
{
    public SendDirectMessageValidator()
    {
        RuleFor(x => x.RecipientUserId).NotEmpty();
        RuleFor(x => x.Content)
            .Must(content => !string.IsNullOrWhiteSpace(content))
            .WithErrorCode("chat.message_empty")
            .WithMessage("Message content is required")
            .MaximumLength(4000)
            .WithErrorCode("chat.message_too_long")
            .WithMessage("Message content must be 4000 characters or fewer");
    }
}
