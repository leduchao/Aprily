using FluentValidation;

namespace Aprily.Backend.Features.Chat.UseCases.SetMessageReaction;

public sealed class SetMessageReactionValidator : AbstractValidator<SetMessageReactionCommand>
{
    public SetMessageReactionValidator()
    {
        RuleFor(command => command.MessageId).NotEmpty();
        RuleFor(command => command.Type)
            .Must(type => type is null || MessageReactionTypes.Allowed.Contains(type))
            .WithErrorCode("chat.invalid_reaction")
            .WithMessage("Reaction type is invalid");
    }
}
