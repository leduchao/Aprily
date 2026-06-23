using Aprily.Backend.Common.Constants;
using Aprily.Backend.Features.Chat.UseCases.GetConversationMessages;
using Aprily.Backend.Features.Chat.UseCases.ListConversations;
using Aprily.Backend.Features.Chat.UseCases.MarkConversationAsRead;
using Aprily.Backend.Features.Chat.UseCases.OpenDirectConversation;
using Aprily.Backend.Features.Chat.UseCases.SendDirectMessage;
using Aprily.Backend.Features.Chat.UseCases.SetMessageReaction;
using Aprily.Backend.Features.Chat.UseCases.SearchConversations;
using Aprily.Backend.Features.Chat.UseCases.CreateGroupConversation;

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
        chat.MapSearchConversationsEndpoint();
        chat.MapGetConversationMessagesEndpoint();
        chat.MapOpenDirectConversationEndpoint();
        chat.MapCreateGroupConversationEndpoint();
        chat.MapSendDirectMessageEndpoint();
        chat.MapSetMessageReactionEndpoint();
        chat.MapMarkConversationAsReadEndpoint();
    }
}
