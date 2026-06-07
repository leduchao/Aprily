using FluentValidation;

namespace Aprily.Application.Chat.SendDirectMessage;

internal sealed class SendDirectMessageCommandValidator : AbstractValidator<SendDirectMessageCommand>
{
    public SendDirectMessageCommandValidator()
    {
        RuleFor(command => command.SenderUserId).NotEmpty();
        RuleFor(command => command.RecipientUserId)
            .NotEmpty()
            .NotEqual(command => command.SenderUserId)
            .WithMessage("A direct message recipient must be another user.");
        RuleFor(command => command.Content)
            .NotEmpty()
            .MaximumLength(4000);
    }
}
