using Aprily.Backend.Common.Extensions;
using Aprily.Backend.Common.Results;
using Aprily.Backend.Database;
using Aprily.Backend.Entities;
using Aprily.Backend.Features.Users.Models;
using Aprily.Backend.Features.Users.Services.Abstractions;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Aprily.Backend.Features.Users.Auth;

public static class SignIn
{
    public record Request(string Email, string Password);
    public record Response(string AccessToken, string RefreshToken, UserBasicInfo User);

    public record Command(string Email, string Password) : IRequest<Result<Response>>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(r => r.Email).NotEmpty().EmailAddress();
            RuleFor(r => r.Password).NotEmpty();
        }
    }

    public class Handler(AppDbContext dbContext, ITokenProvider tokenProvider, IPasswordHasher passwordHasher) : IRequestHandler<Command, Result<Response>>
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ITokenProvider _tokenProvider = tokenProvider;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;

        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(p => p.Email == request.Email, cancellationToken);

            if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                return Result<Response>.Failure(
                    new Error("users.sign_in_failed", "Invalid email or password"));
            }

            user.LastLoginAt = DateTime.UtcNow;

            _dbContext.Users.Update(user);

            var accessToken = _tokenProvider.GenerateAccessToken(user);

            var userProfile = new UserBasicInfo(
                user.EntityId,
                user.Username,
                user.FullName,
                user.Email,
                user.AvatarUrl,
                user.LastLoginAt,
                user.IsEmailVerified
            );

            var refreshToken = _tokenProvider.GenerateRefreshToken();

            var newRefreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
            };

            await _dbContext.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            var response = new Response(accessToken, refreshToken, userProfile);

            return Result<Response>.Success(response);
        }
    }

    public static void MapSignInEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/sign-in", async (Request request, ISender sender) =>
        {
            var command = new Command(request.Email, request.Password);
            var result = await sender.Send(command);

            return result.ToHttpResult();

        }).AllowAnonymous();
    }
}
