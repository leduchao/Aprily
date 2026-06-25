using Aprily.Backend.Common.Extensions;
using MediatR;

namespace Aprily.Backend.Features.Chat.UseCases.DeleteGroupConversation;

public static class DeleteGroupConversationEndpoint
{
    public static void MapDeleteGroupConversationEndpoint(this RouteGroupBuilder group)
    {
        group.MapDelete("/group-conversations/{conversationId:guid}", async (
            Guid conversationId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new DeleteGroupConversationCommand(conversationId), cancellationToken);
            return result.ToHttpResult();
        });
    }
}
