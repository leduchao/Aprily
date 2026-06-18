using Aprily.Backend.Common.Results;
using Aprily.Backend.Database;
using Aprily.Backend.Features.Users.Models;
using Aprily.Backend.Features.Users.Services;

using MediatR;

using Microsoft.EntityFrameworkCore;

using RefreshTokenEntity = Aprily.Backend.Entities.RefreshToken;

namespace Aprily.Backend.Features.Users.UseCases.Auth.RefreshToken;

public record RefreshTokenResponse(string AccessToken, string RefreshToken, UserBasicInfo User);

public sealed class RefreshTokenCommand(string refreshToken) : IRequest<Result<RefreshTokenResponse>>
{
    public string RefreshToken { get; init; } = refreshToken;

    public sealed class Handler(AppDbContext dbContext, ITokenProvider tokenProvider)
        : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ITokenProvider _tokenProvider = tokenProvider;

        public async Task<Result<RefreshTokenResponse>> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            var storedRefreshToken = await _dbContext.RefreshTokens
                .Include(token => token.User)
                .FirstOrDefaultAsync(
                    token => token.Token == request.RefreshToken &&
                        !token.IsDeleted,
                    cancellationToken);

            if (storedRefreshToken is null ||
                storedRefreshToken.IsRevoked ||
                storedRefreshToken.ExpiresAt <= DateTime.UtcNow ||
                storedRefreshToken.User.IsDeleted)
            {
                return Result<RefreshTokenResponse>.Failure(
                    new Error("users.refresh_token_invalid", "Refresh token is invalid or expired"));
            }

            var user = storedRefreshToken.User;
            var newRefreshToken = _tokenProvider.GenerateRefreshToken();
            var now = DateTime.UtcNow;

            storedRefreshToken.IsRevoked = true;
            storedRefreshToken.ReplacedByToken = newRefreshToken;
            storedRefreshToken.UpdatedAt = now;

            await _dbContext.RefreshTokens.AddAsync(
                new RefreshTokenEntity
                {
                    EntityId = Guid.NewGuid(),
                    UserId = user.Id,
                    Token = newRefreshToken,
                    ExpiresAt = now.AddDays(7),
                    IsRevoked = false,
                    CreatedAt = now,
                    UpdatedAt = now,
                    IsDeleted = false
                },
                cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            var accessToken = _tokenProvider.GenerateAccessToken(user);
            var userProfile = new UserBasicInfo(
                user.EntityId,
                user.Username,
                user.FullName,
                user.Email,
                user.AvatarUrl,
                user.LastSignInAt,
                user.IsEmailVerified);

            return Result<RefreshTokenResponse>.Success(
                new RefreshTokenResponse(accessToken, newRefreshToken, userProfile));
        }
    }
}
