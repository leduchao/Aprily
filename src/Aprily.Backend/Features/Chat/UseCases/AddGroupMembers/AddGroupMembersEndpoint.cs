using Aprily.Backend.Common.Extensions;
using MediatR;

namespace Aprily.Backend.Features.Chat.UseCases.AddGroupMembers;

public static class AddGroupMembersEndpoint
{
    private record Request(IReadOnlyList<Guid> MemberUserIds);

    public static void MapAddGroupMembersEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/group-conversations/{conversationId:guid}/members", async (Guid conversationId, Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new AddGroupMembersCommand(conversationId, request.MemberUserIds), cancellationToken);
            return result.ToHttpResult();
        });
    }
}
