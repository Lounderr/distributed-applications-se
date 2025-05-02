using Microsoft.AspNetCore.Identity;

using WildlifeTracker.Data.Models;

namespace WildlifeTracker.Data.Seeding
{
    internal class DummyDataSeeder : ISeeder
    {
        public void Seed(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var animals = new List<Animal>
              {
                  new() { Name = "Leo", Species = "Lion", Age = 5, Weight = 190.5, Height = 120 },
                  new() { Name = "Ella", Species = "Elephant", Age = 10, Weight = 5400.0, Height = 300 },
                  new() { Name = "Max", Species = "Wolf", Age = 3, Weight = 45.3, Height = 80 },
                  new() { Name = "Milo", Species = "Giraffe", Age = 7, Weight = 800.0, Height = 500 },
                  new() { Name = "Zara", Species = "Zebra", Age = 4, Weight = 350.0, Height = 140 }
              };

            dbContext.Animals.AddRange(animals);

            var habitats = new List<Habitat>
            {
               new() { Name = "Savannah", Location = "Africa", Size = 1500.0, Climate = "Dry", AverageTemperature = 30.5 },
               new() { Name = "Rainforest", Location = "Amazon", Size = 5500.0, Climate = "Humid", AverageTemperature = 25.0 },
               new() { Name = "Desert", Location = "Sahara", Size = 9000.0, Climate = "Hot", AverageTemperature = 40.0 },
               new() { Name = "Tundra", Location = "Arctic", Size = 1200.0, Climate = "Cold", AverageTemperature = -10.0 },
               new() { Name = "Grassland", Location = "North America", Size = 3000.0, Climate = "Temperate", AverageTemperature = 20.0 }
            };
            // identity dob

            dbContext.Habitats.AddRange(habitats);

            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            var users = new List<User>
            {
                new() { UserName = "user@mailinator.com", FirstName = "John", LastName = "Doe", DateOfBirth = new DateOnly(1985, 5, 15), City = "Sofia", Email = "user@mailinator.com", EmailConfirmed = true },
                new() { UserName = "curious.observer@mailinator.com", FirstName = "Jane", LastName = "Smith", DateOfBirth = new DateOnly(1990, 8, 20), City = "Plovdiv", Email = "curious.observer@mailinator.com", EmailConfirmed = true }
            };


            foreach (var user in users)
            {
                userManager.CreateAsync(user, "Test123!").GetAwaiter().GetResult();
            }

            var sightings = new List<Sighting>
            {
                new()
                {
                    AnimalId = animals[0].Id,
                    HabitatId = habitats[0].Id,
                    ObserverId = users[0].Id,
                    WeatherConditions = "Sunny",
                    Notes = "The lion was resting under a tree.",
                    SightingDateTime = DateTime.Now.AddDays(-10.1),
                    Animal = animals[0],
                    Habitat = habitats[0],
                    Observer = users[0]
                },
                new()
                {
                    AnimalId = animals[1].Id,
                    HabitatId = habitats[1].Id,
                    ObserverId = users[1].Id,
                    WeatherConditions = "Rainy",
                    Notes = "The elephant was bathing in a river.",
                    SightingDateTime = DateTime.Now.AddDays(-5.3),
                    Animal = animals[1],
                    Habitat = habitats[1],
                    Observer = users[1]
                },
                new()
                {
                    AnimalId = animals[2].Id,
                    HabitatId = habitats[2].Id,
                    ObserverId = users[0].Id,
                    WeatherConditions = "Cloudy",
                    Notes = "The wolf was howling at the moon.",
                    SightingDateTime = DateTime.Now.AddDays(-3.9),
                    Animal = animals[2],
                    Habitat = habitats[2],
                    Observer = users[0]
                },
            };

            dbContext.Sightings.AddRange(sightings);

            dbContext.SaveChanges();
        }
    }

}
