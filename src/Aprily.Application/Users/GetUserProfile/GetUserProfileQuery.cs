using Aprily.SharedKernel;

using MediatR;

namespace Aprily.Application.Users.GetUserProfile;

public record GetUserProfileQuery(string Email) : IRequest<Result<UserProfileResponse>>;

public record UserProfileResponse(
    string Username,
    string? FullName,
    string Email,
    string? AvatarUrl,
    DateTime LastLogin,
    bool IsEmailVerified);


