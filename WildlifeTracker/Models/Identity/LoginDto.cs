using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WildlifeTracker.Models.Identity
{
    public class LoginDto
    {
        [DefaultValue("user@mailinator.com")]
        [EmailAddress]
        [Required]
        public required string Email { get; set; }

        [DefaultValue("Test123!")]
        [MinLength(8)]
        [MaxLength(32)]
        [Required]
        public required string Password { get; set; }
    }
}
