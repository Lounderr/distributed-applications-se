namespace WildlifeTracker.Data.Models
{
    public class Animal : BaseEntity
    {
        public required string Name { get; set; }
        public required string Species { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public string? ImagePath { get; set; }

        public virtual ICollection<Sighting> Sightings { get; set; } = new HashSet<Sighting>();
    }
}
