using Aprily.Backend.Common.Extensions;
using MediatR;

namespace Aprily.Backend.Features.Chat.UseCases.UpdateGroupConversation;

public static class UpdateGroupConversationEndpoint
{
    private record Request(string Name);

    public static void MapUpdateGroupConversationEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("/group-conversations/{conversationId:guid}", async (Guid conversationId, Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new UpdateGroupConversationCommand(conversationId, request.Name), cancellationToken);
            return result.ToHttpResult();
        });
    }
}
