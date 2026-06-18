using Aprily.Backend.Common.Constants;
using Aprily.Backend.Features.Users.UseCases.Auth.RefreshToken;
using Aprily.Backend.Features.Users.UseCases.Auth.SignIn;
using Aprily.Backend.Features.Users.UseCases.Auth.SignOut;
using Aprily.Backend.Features.Users.UseCases.Auth.SignUp;
using Aprily.Backend.Features.Users.UseCases.GetUserProfile;

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

        auth.MapSignUpEndpoint();
        auth.MapSignInEndpoint();
        auth.MapRefreshTokenEndpoint();
        auth.MapSignOutEndpoint();

        users.MapGetUserProfileEndpoint();
    }
}
