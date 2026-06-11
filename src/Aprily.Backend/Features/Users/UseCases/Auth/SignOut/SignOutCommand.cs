using Aprily.Backend.Common.Results;
using Aprily.Backend.Database;
using Aprily.Backend.Features.Users.Services;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Aprily.Backend.Features.Users.UseCases.Auth.SignOut;

public sealed class SignOutCommand : IRequest<Result>
{
    public class SignOutCommandHandler(AppDbContext dbContext, ICurrentUser currentUser, ILogger<SignOutCommandHandler> logger)
        : IRequestHandler<SignOutCommand, Result>
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ICurrentUser _currentUser = currentUser;
        private readonly ILogger<SignOutCommandHandler> _logger = logger;

        public async Task<Result> Handle(SignOutCommand request, CancellationToken cancellationToken)
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
}
