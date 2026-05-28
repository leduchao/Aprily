using Aprily.Application.Abstractions.Services;
using Aprily.Infrastructure.Extensions;

using Microsoft.AspNetCore.Http;

namespace Aprily.Infrastructure.Services;

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public Guid UserEntityId => httpContextAccessor.HttpContext?.User.GetUserEntityId()
        ?? throw new Exception("User Entity Id is invalid");
}
