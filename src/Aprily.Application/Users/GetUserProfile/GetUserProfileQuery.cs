using Aprily.SharedKernel;

using MediatR;

namespace Aprily.Application.Users.GetUserProfile;

public record GetUserProfileQuery(string Email) : IRequest<Result<UserInfoDto>>;

public record UserInfoDto(
    Guid Id,
    string Username,
    string? FullName,
    string Email,
    string? AvatarUrl,
    DateTime LastLogin,
    bool IsEmailVerified);

public record UserProfileDto(
    Guid Id,
    string Username,
    string? FullName,
    string Email,
    string? AvatarUrl,
    DateTime LastLogin,
    bool IsEmailVerified);
