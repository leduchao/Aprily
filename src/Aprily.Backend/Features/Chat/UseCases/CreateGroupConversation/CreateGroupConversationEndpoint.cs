using Aprily.Backend.Common.Extensions;
using MediatR;

namespace Aprily.Backend.Features.Chat.UseCases.CreateGroupConversation;

public static class CreateGroupConversationEndpoint
{
    private record Request(string Name, IReadOnlyList<Guid> MemberUserIds);

    public static void MapCreateGroupConversationEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/group-conversations", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new CreateGroupConversationCommand(request.Name, request.MemberUserIds), cancellationToken);
            return result.ToHttpResult();
        });
    }
}
