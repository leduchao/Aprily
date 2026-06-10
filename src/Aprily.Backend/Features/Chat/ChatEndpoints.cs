using Aprily.Backend.Features.Users.Auth;

namespace Aprily.Backend.Features.Chat;

public static class ChatEndpoints
{
    public static void MapChatEndpoints(this IEndpointRouteBuilder app)
    {
        var chat = app.MapGroup("/chat").WithTags("Chat").RequireAuthorization();
    }
}