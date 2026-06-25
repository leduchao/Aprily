using Aprily.Backend.Common.Extensions;
using MediatR;

namespace Aprily.Backend.Features.Chat.UseCases.RemoveGroupMember;

public static class RemoveGroupMemberEndpoint
{
    public static void MapRemoveGroupMemberEndpoint(this RouteGroupBuilder group)
    {
        group.MapDelete("/group-conversations/{conversationId:guid}/members/{memberUserId:guid}", async (
            Guid conversationId,
            Guid memberUserId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new RemoveGroupMemberCommand(conversationId, memberUserId), cancellationToken);
            return result.ToHttpResult();
        });
    }
}
