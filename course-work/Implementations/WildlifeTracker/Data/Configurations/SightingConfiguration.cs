using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WildlifeTracker.Data.Models;

namespace WildlifeTracker.Data.Configurations
{
    public class SightingConfiguration : IEntityTypeConfiguration<Sighting>
    {
        public void Configure(EntityTypeBuilder<Sighting> builder)
        {
            builder.Property(s => s.Notes)
                .HasMaxLength(500);

            builder.Property(s => s.WeatherConditions)
                .HasMaxLength(100);

            // Do not allow future dates
            builder.ToTable(tb => tb.HasCheckConstraint("CK_Sighting_SightingDateTime", "SightingDateTime <= GETDATE()"));

            builder.HasOne(s => s.Observer)
               .WithMany(u => u.Sightings)
               .HasForeignKey(s => s.ObserverId);

            builder.HasOne(s => s.Animal)
               .WithMany(a => a.Sightings)
               .HasForeignKey(s => s.AnimalId);

            builder.HasOne(s => s.Habitat)
               .WithMany(h => h.Sightings)
               .HasForeignKey(s => s.HabitatId);
        }
    }
}
