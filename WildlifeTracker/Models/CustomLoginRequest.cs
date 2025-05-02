using System.ComponentModel;

namespace WildlifeTracker.Models
{
    public class CustomLoginRequest
    {
        [DefaultValue("user@mailinator.com")]
        public required string Email { get; set; }

        [DefaultValue("Test123!")]
        public required string Password { get; set; }
    }
}
