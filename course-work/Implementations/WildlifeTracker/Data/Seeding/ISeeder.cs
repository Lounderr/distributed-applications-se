namespace WildlifeTracker.Data.Seeding
{
    public interface ISeeder
    {
        void Seed(ApplicationDbContext dbContext, IServiceProvider serviceProvider);
    }
}
