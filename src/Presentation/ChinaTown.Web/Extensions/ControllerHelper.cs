using System.Security.Claims;

namespace ChinaTown.Web.Extensions;

public static class ControllerHelper
{
    public static Guid GetUserIdFromPrincipals(ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    public static string GetRoleFromClaims(ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Role)?.Value!;
    }
}