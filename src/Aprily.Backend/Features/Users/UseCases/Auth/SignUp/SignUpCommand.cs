using Aprily.Backend.Common.Results;
using Aprily.Backend.Database;
using Aprily.Backend.Entities;
using Aprily.Backend.Features.Users.Models;
using Aprily.Backend.Features.Users.Services;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Aprily.Backend.Features.Users.UseCases.Auth.SignUp;

public record SignUpResponse(string AccessToken, string RefreshToken, UserBasicInfo User);

public sealed class SignUpCommand(string? fullName, string username, string email, string password) : IRequest<Result<SignUpResponse>>
{
    public string? FullName { get; init; } = fullName;
    public string Username { get; init; } = username;
    public string Email { get; init; } = email;
    public string Password { get; init; } = password;

    public sealed class Handler(AppDbContext dbContext, ITokenProvider tokenProvider, IPasswordHasher passwordHasher)
        : IRequestHandler<SignUpCommand, Result<SignUpResponse>>
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ITokenProvider _tokenProvider = tokenProvider;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;

        public async Task<Result<SignUpResponse>> Handle(SignUpCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(p => p.Email == request.Email, cancellationToken);

            if (existingUser != null)
            {
                return Result<SignUpResponse>.Failure(
                    new Error("users.sign_up_failed", "A user with this email already exists"));
            }

            var user = new User
            {
                EntityId = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                PasswordHash = _passwordHasher.Hash(request.Password),
                FullName = request.FullName,
                IsEmailVerified = false,
                LastSignInAt = DateTime.UtcNow,
            };

            var accessToken = _tokenProvider.GenerateAccessToken(user);

            var userProfile = new UserBasicInfo(
                user.EntityId,
                user.Username,
                user.FullName,
                user.Email,
                user.AvatarUrl,
                user.LastSignInAt,
                user.IsEmailVerified);

            var refreshToken = _tokenProvider.GenerateRefreshToken();

            var newRefreshToken = new RefreshToken
            {
                EntityId = Guid.NewGuid(),
                User = user,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
            };

            await _dbContext.Users.AddAsync(user, cancellationToken);
            await _dbContext.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            var response = new SignUpResponse(accessToken, refreshToken, userProfile);

            return Result<SignUpResponse>.Success(response);
        }
    }
}
