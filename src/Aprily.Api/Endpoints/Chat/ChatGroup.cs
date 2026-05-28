namespace Aprily.Api.Endpoints.Chat;

public static class ChatGroup
{
    public static RouteGroupBuilder MapChat(this IEndpointRouteBuilder app)
    {
        return app.MapGroup($"{BaseApiEndpoint.BasePath}/chat")
            .WithTags("Chat")
            .RequireAuthorization();
    }
}
