using Aprily.Backend.Common.Extensions;
using MediatR;

namespace Aprily.Backend.Features.Chat.UseCases.LeaveGroupConversation;

public static class LeaveGroupConversationEndpoint
{
    public static void MapLeaveGroupConversationEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/group-conversations/{conversationId:guid}/leave", async (
            Guid conversationId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new LeaveGroupConversationCommand(conversationId), cancellationToken);
            return result.ToHttpResult();
        });
    }
}
