namespace WildlifeTracker.Data.Seeding
{
    public class ApplicationDbContextSeeder
    {
        public void SeedDatabase(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var logger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger(typeof(ApplicationDbContextSeeder));

            if (logger == null)
            {
                throw new Exception("Logger not found in the service provider.");
            }

            if (dbContext.Users.Any())
            {
                logger.LogInformation($"Seeding aborted - the has already been seeded.");
                return;
            }

            var seeders = new List<ISeeder>
                          {
                              new DummyDataSeeder(),
                          };

            foreach (var seeder in seeders)
            {
                logger.LogInformation($"Seeder {seeder.GetType().Name} started seeding data.");
                seeder.Seed(dbContext, serviceProvider);
                logger.LogInformation($"Seeder {seeder.GetType().Name} finished seeding data sucessfully.");
            }

        }
    }
}
