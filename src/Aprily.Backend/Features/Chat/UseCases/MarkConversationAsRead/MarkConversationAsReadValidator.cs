using FluentValidation;

namespace Aprily.Backend.Features.Chat.UseCases.MarkConversationAsRead;

public sealed class MarkConversationAsReadValidator : AbstractValidator<MarkConversationAsReadCommand>
{
    public MarkConversationAsReadValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty();
        RuleFor(x => x.MessageId).NotEmpty();
    }
}
