using Aprily.Backend.Common.Constants;
using Aprily.Backend.Features.Users.Auth;

namespace Aprily.Backend.Features.Users;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var users = app
            .MapGroup($"{ApiPath.BasePath}/users")
            .WithTags("Users")
            .RequireAuthorization();

        var auth = users
            .MapGroup("/auth")
            .WithTags("Users Auth");

        auth.MapSignInEndpoint();
        auth.MapSignUpEndpoint();
    }
}