using Microsoft.AspNetCore.Identity;

namespace WildlifeTracker.Data.Models
{
    public class User : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? City { get; set; }

        public virtual ICollection<Sighting> Sightings { get; set; } = new HashSet<Sighting>();
    }
}
