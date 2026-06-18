using FluentValidation;

namespace Aprily.Backend.Features.Chat.UseCases.GetConversationMessages;

public sealed class GetConversationMessagesValidator : AbstractValidator<GetConversationMessagesQuery>
{
    public GetConversationMessagesValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty();
        RuleFor(x => x.Take).InclusiveBetween(1, 100);
    }
}
