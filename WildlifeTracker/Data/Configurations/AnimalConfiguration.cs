using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WildlifeTracker.Data.Models;

namespace WildlifeTracker.Data.Configurations
{
    public class AnimalConfiguration : IEntityTypeConfiguration<Animal>
    {
        public void Configure(EntityTypeBuilder<Animal> builder)
        {
            builder.Property(a => a.Name)
                .HasMaxLength(100);

            builder.Property(a => a.Species)
                .HasMaxLength(100);

            builder.Property(a => a.Weight)
                .HasPrecision(5, 2);

            builder.Property(a => a.Height)
                .HasPrecision(5, 2);

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_Animal_Age_Positive", "[Age] >= 0");
                tb.HasCheckConstraint("CK_Animal_Height_Positive", "[Height] > 0");
                tb.HasCheckConstraint("CK_Animal_Weight_Positive", "[Weight] > 0");
            });
        }
    }
}
