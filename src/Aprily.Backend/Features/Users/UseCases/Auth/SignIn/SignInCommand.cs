using Aprily.Backend.Common.Results;
using Aprily.Backend.Database;
using Aprily.Backend.Features.Users.Models;
using Aprily.Backend.Features.Users.Services;

using MediatR;

using Microsoft.EntityFrameworkCore;

using RefreshTokenEntity = Aprily.Backend.Entities.RefreshToken;

namespace Aprily.Backend.Features.Users.UseCases.Auth.SignIn;

public record SignInResponse(string AccessToken, string RefreshToken, UserBasicInfo User);

public sealed class SignInCommand(string email, string password) : IRequest<Result<SignInResponse>>
{
    public string Email { get; init; } = email;
    public string Password { get; init; } = password;


    public class SignInCommandHandler(AppDbContext dbContext, ITokenProvider tokenProvider, IPasswordHasher passwordHasher)
        : IRequestHandler<SignInCommand, Result<SignInResponse>>
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ITokenProvider _tokenProvider = tokenProvider;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;

        public async Task<Result<SignInResponse>> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(p => p.Email == request.Email, cancellationToken);

            if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                return Result<SignInResponse>.Failure(
                    new Error("users.sign_in_failed", "Invalid email or password"));
            }

            user.LastSignInAt = DateTime.UtcNow;

            var accessToken = _tokenProvider.GenerateAccessToken(user);

            var userProfile = new UserBasicInfo(
                user.EntityId,
                user.Username,
                user.FullName,
                user.Email,
                user.AvatarUrl,
                user.LastSignInAt,
                user.IsEmailVerified
            );

            var refreshToken = _tokenProvider.GenerateRefreshToken();

            var newRefreshToken = new RefreshTokenEntity
            {
                EntityId = Guid.NewGuid(),
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
            };

            await _dbContext.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            var response = new SignInResponse(accessToken, refreshToken, userProfile);

            return Result<SignInResponse>.Success(response);
        }
    }
}
