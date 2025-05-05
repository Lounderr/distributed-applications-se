using WildlifeTracker.Data.Models;
using WildlifeTracker.Exceptions;

namespace WildlifeTracker.Services
{
    public class ResourceAccessService(ICurrentUserService user) : IResourceAccessService
    {
        public void Authorize(BaseEntity? entity)
        {
            if (user.IsAdmin)
                return;

            this.Authorize(user.UserId, entity);

            return;
        }

        public void Authorize(string? userId, BaseEntity? entity)
        {
            if (userId == null)
            {
                throw new UnauthorizedException("User not authenticated");
            }

            if (entity == null || entity.CreatedBy == userId)
            {
                return;
            }
            else
            {
                throw new AccessDeniedException("You do not have permission to access this resource");
            }
        }
    }
}
