using System.Security.Claims;

namespace Aprily.Backend.Common.Extensions;

public static class ClaimsPrincipleExtension
{
    public static Guid GetUserEntityId(this ClaimsPrincipal claimsPrincipal)
    {
        var userEntityId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        var parseRessult = Guid.TryParse(userEntityId, out Guid value);

        return parseRessult ? value : throw new ApplicationException("User Entity Id is invalid");
    }
}
