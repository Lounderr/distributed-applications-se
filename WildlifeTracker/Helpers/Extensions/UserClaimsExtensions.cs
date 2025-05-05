using System.Security.Claims;

namespace WildlifeTracker.Helpers.Extensions
{
    public static class UserClaimsExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal? user)
        {
            return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        public static string? GetUserName(this ClaimsPrincipal? user)
        {
            return user?.FindFirst(ClaimTypes.Name)?.Value;
        }
        public static string? GetUserEmail(this ClaimsPrincipal? user)
        {
            return user?.FindFirst(ClaimTypes.Email)?.Value;
        }
        public static IEnumerable<string> GetRoles(this ClaimsPrincipal? user)
        {
            return user?.FindAll(ClaimTypes.Role)?.Select(r => r.Value) ?? [];
        }
    }
}
