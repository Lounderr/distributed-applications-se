using WildlifeTracker.Helpers.Extensions;
using WildlifeTracker.Services;

namespace WildlifeTracker.Middleware
{
    public class UserActivityMiddleware(RequestDelegate next, IOnlineUsersService onlineUsersService)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User?.Identity?.IsAuthenticated ?? false)
            {
                var userId = context.User?.GetUserId();
                var username = context.User?.GetUserName();

                if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(username))
                {
                    onlineUsersService.ReportActivity(userId, username);
                }
            }

            await next(context);
        }
    }
}
