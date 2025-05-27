namespace WildlifeTracker.Services
{
    public interface IOnlineUsersService
    {
        IEnumerable<string> GetOnlineUsers();
        void ReportActivity(string userId, string username);
    }
}