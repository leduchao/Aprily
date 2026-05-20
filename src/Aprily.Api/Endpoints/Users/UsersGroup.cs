namespace Aprily.Api.Endpoints.Users;

public static class UsersGroup
{
    public static RouteGroupBuilder MapUsers(this IEndpointRouteBuilder app)
    {
        return app.MapGroup($"{BaseApiEndpoint.BasePath}/users")
            .WithTags("Users")
            .RequireAuthorization();
    }
}