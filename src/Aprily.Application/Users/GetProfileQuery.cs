using Aprily.Application.Abstractions.Cqrs;
using Aprily.SharedKernel;

namespace Aprily.Application.Users;

public record GetProfileQuery(string Email) : IQuery<UserProfileResponse>;

public record UserProfileResponse(string Username, string FullName, string? AvatarUrl, DateTime LastLogin);

public class GetProfileQueryHandler : IQueryHandler<GetProfileQuery, UserProfileResponse>
{
    public async Task<Result<UserProfileResponse>> Handle(GetProfileQuery query, CancellationToken ct)
    {
        await Task.CompletedTask;

        return Result<UserProfileResponse>.Success(new UserProfileResponse(
            query.Email, 
            query.Email, 
            null, 
            DateTime.UtcNow));
    }
}
