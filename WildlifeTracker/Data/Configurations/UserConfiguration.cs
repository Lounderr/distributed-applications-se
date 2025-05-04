using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WildlifeTracker.Data.Models;

namespace WildlifeTracker.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.FirstName)
                .HasMaxLength(50);

            builder.Property(u => u.LastName)
                .HasMaxLength(50);

            builder.Property(u => u.City)
                .HasMaxLength(100);

            builder.Property(u => u.UserName)
                .IsUnicode(false);

            builder.Property(u => u.NormalizedUserName)
                .IsUnicode(false);

            builder.Property(u => u.Email)
                .IsUnicode(false);

            builder.Property(u => u.NormalizedEmail)
                .IsUnicode(false);

            builder.Property(u => u.PhoneNumber)
                .IsUnicode(false)
                .HasMaxLength(16);

            builder.ToTable(tb =>
            {
                // Enforce E164 format for storing phone numbers  
                tb.HasCheckConstraint("CK_User_PhoneNumber_E164",
                "PhoneNumber LIKE '+%' AND " +
                "LEN(PhoneNumber) >= 2 AND LEN(PhoneNumber) <= 16 AND " +
                "PhoneNumber NOT LIKE '%[^+0-9]%'");

                // Check constraint to ensure users are at least 14 years old  
                tb.HasCheckConstraint("CK_User_AdultOnly", "DateOfBirth <= DATEADD(YEAR, -14, GETDATE())");

                // Check constraint to ensure DateOfBirth is not in the future  
                tb.HasCheckConstraint("CK_User_DateOfBirth_NotFuture", "DateOfBirth <= CAST(GETDATE() AS DATE)");

                // Check constraint to ensure DateOfBirth is not more than 120 years in the past  
                tb.HasCheckConstraint("CK_User_DateOfBirth_MaxAge", "DateOfBirth >= DATEADD(YEAR, -120, CAST(GETDATE() AS DATE))");
            });
        }
    }
}
