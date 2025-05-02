using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WildlifeTracker.Data.Models;

namespace WildlifeTracker.Data.Configurations
{
    public class HabitatConfiguration : IEntityTypeConfiguration<Habitat>
    {
        public void Configure(EntityTypeBuilder<Habitat> builder)
        {
            builder.Property(h => h.Name)
                .HasMaxLength(100);

            builder.Property(h => h.Location)
                .HasMaxLength(100);

            builder.Property(h => h.Climate)
                .HasMaxLength(100);

            builder.Property(h => h.Size)
               .HasPrecision(5, 2);

            builder.Property(h => h.AverageTemperature)
                .HasPrecision(5, 2);


            builder.ToTable(tb => tb.HasCheckConstraint("CK_Habitat_Size_Positive", "[Size] > 0"));
        }
    }
}
