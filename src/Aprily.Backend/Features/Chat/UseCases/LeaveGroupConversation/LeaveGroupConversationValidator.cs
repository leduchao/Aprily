using FluentValidation;

namespace Aprily.Backend.Features.Chat.UseCases.LeaveGroupConversation;

public sealed class LeaveGroupConversationValidator : AbstractValidator<LeaveGroupConversationCommand>
{
    public LeaveGroupConversationValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty();
    }
}
