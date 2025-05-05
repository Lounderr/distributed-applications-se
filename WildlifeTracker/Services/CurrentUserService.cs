using WildlifeTracker.Constants;
using WildlifeTracker.Exceptions;
using WildlifeTracker.Helpers.Extensions;

namespace WildlifeTracker.Services
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        public string UserId =>
            httpContextAccessor.HttpContext?.User?.GetUserId() ?? throw new UnauthorizedException("User ID is null");

        public string Email =>
            httpContextAccessor.HttpContext?.User?.GetUserEmail() ?? throw new UnauthorizedException("User email is null");

        public string UserName =>
            httpContextAccessor.HttpContext?.User?.GetUserName() ?? throw new UnauthorizedException("Username is null");

        public bool IsAdmin =>
            httpContextAccessor.HttpContext?.User?.GetRoles().Contains(RoleConstants.Admin) ?? false;

        public bool IsAuthenticated =>
            httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
