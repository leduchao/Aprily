using Aprily.Backend.Common.Extensions;

namespace Aprily.Backend.Features.Users.Services.Implements;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid UserEntityId => _httpContextAccessor.HttpContext?.User.GetUserEntityId()
        ?? throw new Exception("User Entity Id is invalid");
}
