using FluentValidation;

namespace Aprily.Backend.Features.Chat.UseCases.CreateGroupConversation;

public sealed class CreateGroupConversationValidator : AbstractValidator<CreateGroupConversationCommand>
{
    public CreateGroupConversationValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.MemberUserIds).Must(ids => ids.Distinct().Count() >= 2)
            .WithMessage("Select at least two friends");
    }
}
