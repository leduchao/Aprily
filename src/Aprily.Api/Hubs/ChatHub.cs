using Aprily.Application.Chat;
using Aprily.Application.Chat.GetDirectMessages;
using Aprily.Application.Chat.SendDirectMessage;
using Aprily.Infrastructure.Extensions;
using Aprily.SharedKernel;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Aprily.Api.Hubs;

[Authorize]
public sealed class ChatHub(ISender sender) : Hub<IChatClient>
{
    private readonly ISender _sender = sender;

    public async Task<ChatMessageResponse> SendDirectMessage(
        Guid recipientUserId,
        string content)
    {
        Guid senderUserId = Context.User!.GetUserEntityId();

        var command = new SendDirectMessageCommand(senderUserId, recipientUserId, content);
        Result<ChatMessageResponse> result = await _sender.Send(command, Context.ConnectionAborted);
        if (result.IsFailure || result.Data is null)
        {
            throw new HubException(result.Error?.Message ?? "Unable to send the message.");
        }

        await Clients
            .Users(senderUserId.ToString(), recipientUserId.ToString())
            .ReceiveDirectMessage(result.Data);

        return result.Data;
    }

    public async Task<IReadOnlyList<ChatMessageResponse>> GetDirectMessages(
        Guid otherUserId,
        int take = 50,
        DateTime? before = null)
    {
        Guid currentUserId = Context.User!.GetUserEntityId();

        var query = new GetDirectMessagesQuery(currentUserId, otherUserId, take, before);
        Result<IReadOnlyList<ChatMessageResponse>> result = await _sender.Send(
            query,
            Context.ConnectionAborted);
        if (result.IsFailure || result.Data is null)
        {
            throw new HubException(result.Error?.Message ?? "Unable to load messages.");
        }

        return result.Data;
    }
}
