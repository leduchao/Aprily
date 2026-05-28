namespace Aprily.Api.Endpoints.Chat;

public class Test : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapChat().MapGet("/message", () => Results.Ok("Test refresh token"));
    }
}
