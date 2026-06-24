using FluentValidation;

namespace Aprily.Backend.Features.Chat.UseCases.AddGroupMembers;

public sealed class AddGroupMembersValidator : AbstractValidator<AddGroupMembersCommand>
{
    public AddGroupMembersValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty();
        RuleFor(x => x.MemberUserIds).NotEmpty().Must(ids => ids.Distinct().Count() <= 50);
    }
}
