using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using WildlifeTracker.Data.Models;

namespace WildlifeTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Animal> Animals { get; set; }
        public DbSet<Habitat> Habitats { get; set; }
        public DbSet<Sighting> Sightings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Change schema and names of ASP.NET Core Identity tables  
            this.ConfigureIdentityTables(modelBuilder);

            modelBuilder.Entity<Animal>()
                .Property(a => a.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Animal>()
                .Property(a => a.Species)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Habitat>()
                .Property(h => h.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Habitat>()
                .Property(h => h.Location)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Habitat>()
                .Property(h => h.Climate)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Sighting>()
                .Property(s => s.ObserverName)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Sighting>()
              .Property(s => s.Notes)
              .HasMaxLength(100)
              .IsRequired();
        }

        private void ConfigureIdentityTables(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityUser>(b => b.ToTable("Users", "identity"));
            modelBuilder.Entity<IdentityRole>(b => b.ToTable("Roles", "identity"));
            modelBuilder.Entity<IdentityUserRole<string>>(b => b.ToTable("UserRoles", "identity"));
            modelBuilder.Entity<IdentityUserClaim<string>>(b => b.ToTable("UserClaims", "identity"));
            modelBuilder.Entity<IdentityUserLogin<string>>(b => b.ToTable("UserLogins", "identity"));
            modelBuilder.Entity<IdentityRoleClaim<string>>(b => b.ToTable("RoleClaims", "identity"));
            modelBuilder.Entity<IdentityUserToken<string>>(b => b.ToTable("UserTokens", "identity"));
        }
    }
}
