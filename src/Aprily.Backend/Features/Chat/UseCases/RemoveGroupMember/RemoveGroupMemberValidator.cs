using FluentValidation;

namespace Aprily.Backend.Features.Chat.UseCases.RemoveGroupMember;

public sealed class RemoveGroupMemberValidator : AbstractValidator<RemoveGroupMemberCommand>
{
    public RemoveGroupMemberValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty();
        RuleFor(x => x.MemberUserId).NotEmpty();
    }
}
