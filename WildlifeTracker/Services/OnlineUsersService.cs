
using Microsoft.Extensions.Caching.Memory;

using WildlifeTracker.Constants;
using WildlifeTracker.Helpers.Extensions;

namespace WildlifeTracker.Services
{
    public class OnlineUsersService(IMemoryCache memoryCache) : IOnlineUsersService
    {
        public IEnumerable<string> GetOnlineUsers()
        {
            if (memoryCache is MemoryCache cacheEntries)
            {
                List<string> usernames = cacheEntries
                   .Keys
                   .OfType<string>()
                   .Where(key => key.StartsWith("UserActivity_"))
                   .Shuffle()
                   .Take(50)
                   .Select(key => memoryCache.Get<string>(key) ?? throw new ArgumentNullException("Onlune users cache contains null values"))
                   .ToList();

                return usernames;
            }
            else
            {
                throw new InvalidOperationException("Memory cache is not available");
            }
        }

        public void ReportActivity(string userId, string username)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException(ErrorCodes.ArgumentNullOrEmpty, $"Parameter {nameof(userId)} cannot be null");
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException(ErrorCodes.ArgumentNullOrEmpty, $"Parameter {nameof(username)} cannot be null");
            }

            var cacheKey = $"UserActivity_{userId}";
            memoryCache.Set(cacheKey, username, TimeSpan.FromMinutes(30));
        }
    }
}
