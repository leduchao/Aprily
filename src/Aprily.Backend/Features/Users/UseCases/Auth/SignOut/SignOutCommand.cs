using Aprily.Backend.Common.Results;
using Aprily.Backend.Database;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Aprily.Backend.Features.Users.UseCases.Auth.SignOut;

public sealed class SignOutCommand(string? refreshToken, Guid? userEntityId) : IRequest<Result>
{
    public string? RefreshToken { get; init; } = refreshToken;
    public Guid? UserEntityId { get; init; } = userEntityId;

    public class SignOutCommandHandler(AppDbContext dbContext, ILogger<SignOutCommandHandler> logger)
        : IRequestHandler<SignOutCommand, Result>
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ILogger<SignOutCommandHandler> _logger = logger;

        public async Task<Result> Handle(SignOutCommand request, CancellationToken cancellationToken)
        {
            var userId = await GetUserId(request, cancellationToken);
            if (userId is null)
            {
                return Result.Success();
            }

            var revokedTokens = await RevokeAllUserRefreshTokensAsync(userId.Value, cancellationToken);
            _logger.LogInformation("Revoked {RevokedTokens} tokens", revokedTokens);

            return Result.Success();
        }

        private async Task<int?> GetUserId(SignOutCommand request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                var refreshToken = await _dbContext.RefreshTokens
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        token => token.Token == request.RefreshToken &&
                            !token.IsDeleted,
                        cancellationToken);

                if (refreshToken is not null)
                {
                    return refreshToken.UserId;
                }
            }

            if (request.UserEntityId is null)
            {
                return null;
            }

            return await _dbContext.Users
                .Where(user => user.EntityId == request.UserEntityId && !user.IsDeleted)
                .Select(user => (int?)user.Id)
                .FirstOrDefaultAsync(cancellationToken);
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
}
