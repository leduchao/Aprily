using Aprily.Backend.Common.Extensions;
using Aprily.Backend.Common.Results;
using Aprily.Backend.Database;
using Aprily.Backend.Features.Users.Services.Abstractions;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Aprily.Backend.Features.Users.Auth;

public static class SignOut
{
    public record Request();
    public record Response();

    internal record Command() : IRequest<Result>;

    internal sealed class Handler(AppDbContext dbContext, ICurrentUser currentUser, ILogger<Handler> logger) : IRequestHandler<Command, Result>
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ICurrentUser _currentUser = currentUser;
        private readonly ILogger<Handler> _logger = logger;

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var currentUser = await _dbContext.Users.FirstOrDefaultAsync(p => p.EntityId == _currentUser.UserEntityId, cancellationToken);
            if (currentUser is null)
            {
                return Result.Failure(new Error("users.user_notFound", "User is invalid"));
            }

            var revokedTokens = await RevokeAllUserRefreshTokensAsync(currentUser.Id, cancellationToken);
            _logger.LogInformation("Revoked {RevokedTokens} tokens", revokedTokens);

            return Result.Success();
        }

        private async Task<int> RevokeAllUserRefreshTokensAsync(int userId, CancellationToken cancellationToken)
        {
            return await _dbContext.RefreshTokens
                .Where(r => r.UserId == userId && r.IsRevoked == false)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(r => r.IsRevoked, true)
                        .SetProperty(p => p.UpdatedAt, DateTime.UtcNow)
                        .SetProperty(p => p.IsDeleted, true),
                    cancellationToken);
        }
    }

    internal static void MapSignOutEndpoint(this RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost("/sign-out", async (HttpContext httpContext, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new Command(), cancellationToken);

            httpContext.Response.Cookies.Delete("refreshToken");

            return result.ToHttpResult();
        });
    }
}
