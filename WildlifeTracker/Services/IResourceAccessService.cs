using WildlifeTracker.Data.Models;

namespace WildlifeTracker.Services
{
    public interface IResourceAccessService
    {
        void Authorize(BaseEntity? entity);
        void Authorize(string? userId, BaseEntity? entity);
    }
}