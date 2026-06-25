using FluentValidation;

namespace Aprily.Backend.Features.Chat.UseCases.UpdateGroupConversation;

public sealed class UpdateGroupConversationValidator : AbstractValidator<UpdateGroupConversationCommand>
{
    public UpdateGroupConversationValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.AvatarUrl).MaximumLength(2048).When(x => x.HasAvatarUrl);
    }
}
