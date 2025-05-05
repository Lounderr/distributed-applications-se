namespace WildlifeTracker.Services
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string Email { get; }
        string UserName { get; }
        bool IsAuthenticated { get; }
        bool IsAdmin { get; }
    }
}