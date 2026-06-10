using Aprily.Backend.Common.Constants;

namespace Aprily.Backend.Features.Chat;

public static class ChatEndpoints
{
    public static void MapChatEndpoints(this IEndpointRouteBuilder app)
    {
        var chat = app
            .MapGroup($"{ApiPath.BasePath}/chat")
            .WithTags("Chat")
            .RequireAuthorization();
    }
}