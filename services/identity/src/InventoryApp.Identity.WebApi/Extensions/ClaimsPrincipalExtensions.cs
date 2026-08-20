using System.Security.Claims;

namespace InventoryApp.Identity.WebApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var id = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(id!);
    }

    public static string GetUsername(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
    }

    public static IList<string> GetRoles(this ClaimsPrincipal principal)
    {
        return principal.FindAll(ClaimTypes.Role).Select(c => c.Value).Distinct().ToList();
    }

    public static IList<string> GetPermissions(this ClaimsPrincipal principal)
    {
        return principal.FindAll("permission").Select(c => c.Value).Distinct().ToList();
    }
}
