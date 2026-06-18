namespace Aprily.Backend.Features.Users.UseCases.Auth;

internal static class AuthCookieOptions
{
    public const string RefreshTokenCookieName = "refreshToken";

    public static CookieOptions CreateRefreshTokenCookieOptions(HttpRequest request, DateTimeOffset expires)
    {
        var isHttps = request.IsHttps;

        return new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
            Expires = expires
        };
    }

    public static CookieOptions CreateDeleteRefreshTokenCookieOptions(HttpRequest request)
    {
        var options = CreateRefreshTokenCookieOptions(request, DateTimeOffset.UnixEpoch);
        options.Expires = DateTimeOffset.UnixEpoch;

        return options;
    }
}
