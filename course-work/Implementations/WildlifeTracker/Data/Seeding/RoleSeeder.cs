using Microsoft.AspNetCore.Identity;

using WildlifeTracker.Constants;

namespace WildlifeTracker.Data.Seeding
{
    internal class RoleSeeder : ISeeder
    {
        public void Seed(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var role = new IdentityRole(RoleConstants.Admin);
            roleManager.CreateAsync(role).GetAwaiter().GetResult();
        }
    }

}
