using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using WildlifeTracker.Helpers.DataAnotations;

namespace WildlifeTracker.Models.Identity
{
    public class RegisterDto : LoginDto
    {
        [DefaultValue("John")]
        [MaxLength(50)]
        [Required]
        public required string FirstName { get; set; }

        [DefaultValue("Smith")]
        [MaxLength(50)]
        [Required]
        public required string LastName { get; set; }

        [DefaultValue("+359 876 455 303")]
        [E164FormatValidator]
        [Required]
        public required string PhoneNumber { get; set; }

        [DefaultValue("Plovdiv")]
        [MaxLength(50)]
        public string? City { get; set; }

        [DefaultValue("2004-02-13")]
        [DateOfBirthValidator]
        [Required]
        public DateOnly DateOfBirth { get; set; }
    }
}
