using Aprily.SharedKernel;

using MediatR;

namespace Aprily.Application.Users.Auth.RefreshToken;

public record RefreshTokenCommand(string Token) : IRequest<Result<RefreshTokenResponse>>;

public record RefreshTokenResponse(string AccessToken, string RefreshToken);
