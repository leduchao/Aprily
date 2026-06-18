using Aprily.Backend.Common.Constants;
using Aprily.Backend.Features.Chat.UseCases.GetConversationMessages;
using Aprily.Backend.Features.Chat.UseCases.ListConversations;
using Aprily.Backend.Features.Chat.UseCases.MarkConversationAsRead;
using Aprily.Backend.Features.Chat.UseCases.OpenDirectConversation;
using Aprily.Backend.Features.Chat.UseCases.SendDirectMessage;

namespace Aprily.Backend.Features.Chat;

public static class ChatEndpoints
{
    public static void MapChatEndpoints(this IEndpointRouteBuilder app)
    {
        var chat = app
            .MapGroup($"{ApiPath.BasePath}/chat")
            .WithTags("Chat")
            .RequireAuthorization();

        chat.MapListConversationsEndpoint();
        chat.MapGetConversationMessagesEndpoint();
        chat.MapOpenDirectConversationEndpoint();
        chat.MapSendDirectMessageEndpoint();
        chat.MapMarkConversationAsReadEndpoint();
    }
}
