using System.Reflection;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using WildlifeTracker.Data.Models;
using WildlifeTracker.Data.Models.Interfaces;

namespace WildlifeTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        private static readonly MethodInfo SetIsDeletedQueryFilterMethod =
            typeof(ApplicationDbContext).GetMethod(
                nameof(SetIsDeletedQueryFilter),
                BindingFlags.NonPublic | BindingFlags.Static)!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Animal> Animals { get; set; }
        public DbSet<Habitat> Habitats { get; set; }
        public DbSet<Sighting> Sightings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

            var entityTypes = modelBuilder.Model.GetEntityTypes().ToList();

            // Set global query filter for not deleted entities only
            var deletableEntityTypes = entityTypes
                .Where(et => et.ClrType != null && typeof(IDeletableEntity).IsAssignableFrom(et.ClrType));
            foreach (var deletableEntityType in deletableEntityTypes)
            {
                var method = SetIsDeletedQueryFilterMethod.MakeGenericMethod(deletableEntityType.ClrType);
                method.Invoke(null, new object[] { modelBuilder });
            }

            // Disable cascade delete
            var foreignKeys = entityTypes
                .SelectMany(e => e.GetForeignKeys().Where(f => f.DeleteBehavior == DeleteBehavior.Cascade));
            foreach (var foreignKey in foreignKeys)
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        public override int SaveChanges()
        {
            this.UpdateAuditInfo();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            this.UpdateAuditInfo();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditInfo()
        {
            this.ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified))
                .ToList()
                .ForEach(entry =>
                {
                    var entity = (IAuditInfo)entry.Entity;
                    if (entry.State == EntityState.Added)
                    {
                        entity.CreatedOn = DateTime.UtcNow;
                    }
                    else
                    {
                        entity.ModifiedOn = DateTime.UtcNow;
                    }
                });
        }

        private static void SetIsDeletedQueryFilter<T>(ModelBuilder builder)
            where T : class, IDeletableEntity
            => builder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);

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
