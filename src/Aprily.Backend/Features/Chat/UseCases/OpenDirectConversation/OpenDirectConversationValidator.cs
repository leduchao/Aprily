using FluentValidation;

namespace Aprily.Backend.Features.Chat.UseCases.OpenDirectConversation;

public sealed class OpenDirectConversationValidator : AbstractValidator<OpenDirectConversationCommand>
{
    public OpenDirectConversationValidator()
    {
        RuleFor(x => x.RecipientUserId).NotEmpty();
    }
}
