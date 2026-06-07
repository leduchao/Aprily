using Aprily.Application.Chat;

namespace Aprily.Api.Hubs;

public interface IChatClient
{
    Task ReceiveDirectMessage(ChatMessageResponse message);
}
