using FluentValidation;

namespace Aprily.Backend.Features.Chat.UseCases.DeleteGroupConversation;

public sealed class DeleteGroupConversationValidator : AbstractValidator<DeleteGroupConversationCommand>
{
    public DeleteGroupConversationValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty();
    }
}
