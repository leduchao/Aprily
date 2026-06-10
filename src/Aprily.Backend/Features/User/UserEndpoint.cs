
using Aprily.Backend.Features.User.Auth;

namespace Aprily.Backend.Features.User;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users").WithTags("Users").RequireAuthorization();

        group.MapSignInEndpoint();
        group.MapSignUpEndpoint();
    }
}