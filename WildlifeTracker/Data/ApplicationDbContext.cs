using Microsoft.EntityFrameworkCore;

using YourNamespace.Data.Models;

namespace WildlifeTracker.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Animal> Animals { get; set; }
        public DbSet<Habitat> Habitats { get; set; }
        public DbSet<Sighting> Sightings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Animal>()
                .Property(a => a.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Habitat>()
                .Property(h => h.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Sighting>()
                .Property(s => s.ObserverName)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}
