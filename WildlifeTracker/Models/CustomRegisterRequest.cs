using System.ComponentModel;

namespace WildlifeTracker.Models
{
    public class CustomRegisterRequest
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        [DefaultValue("user@mailinator.com")]
        public required string Email { get; set; }

        [DefaultValue("Test123!")]
        public required string Password { get; set; }
        public string? City { get; set; }
        public DateOnly DateOfBirth { get; set; }
    }
}
