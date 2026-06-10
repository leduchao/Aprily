using Aprily.Backend.Common.Results;
using Aprily.Backend.Database;
using Aprily.Backend.Entities;
using Aprily.Backend.Features.Users.Models;
using Aprily.Backend.Features.Users.Services.Abstractions;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Aprily.Backend.Features.Users.Auth;

public static class SignUp
{
    public record Request(string? FullName, string Username, string Email, string Password);
    public record Response(string AccessToken, string RefreshToken, UserBasicInfo User);

    internal record Command(string? FullName, string Username, string Email, string Password) : IRequest<Result<Response>>;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Username).NotEmpty().MinimumLength(3).MaximumLength(50);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(16);
            RuleFor(x => x.FullName).MaximumLength(100);
        }
    }

    internal sealed class Handler(AppDbContext dbContext, ITokenProvider tokenProvider, IPasswordHasher passwordHasher) : IRequestHandler<Command, Result<Response>>
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ITokenProvider _tokenProvider = tokenProvider;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;

        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(p => p.Email == request.Email, cancellationToken);

            if (existingUser != null)
            {
                return Result<Response>.Failure(
                    new Error("users.sign_up_failed", "A user with this email already exists"));
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = _passwordHasher.Hash(request.Password),
                FullName = request.FullName,
                IsEmailVerified = false,
                LastLoginAt = DateTime.UtcNow
            };

            var accessToken = _tokenProvider.GenerateAccessToken(user);

            var userProfile = new UserBasicInfo(
                user.EntityId,
                user.Username,
                user.FullName,
                user.Email,
                user.AvatarUrl,
                user.LastLoginAt,
                user.IsEmailVerified);

            var refreshToken = _tokenProvider.GenerateRefreshToken();

            var newRefreshToken = new RefreshToken
            {
                User = user,
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
            };

            await _dbContext.Users.AddAsync(user, cancellationToken);
            await _dbContext.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            var response = new Response(accessToken, refreshToken, userProfile);

            return Result<Response>.Success(response);
        }
    }

    public static void MapSignUpEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/sign-up", async (Request request, ISender sender, HttpContext httpContext) =>
        {
            var command = new Command(request.FullName, request.Username, request.Email, request.Password);
            var result = await sender.Send(command);
            if (result.IsFailure || result.Data is null)
            {
                return Results.BadRequest(result);
            }

            httpContext.Response.Cookies.Append(
                "refreshToken",
                result.Data.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });

            return Results.Ok(Result<object>.Success(new
            {
                accessToken = result.Data.AccessToken,
                user = result.Data.User
            }));
        }).AllowAnonymous();
    }
}
