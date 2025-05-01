namespace WildlifeTracker.Data.Models
{
    public class Sighting
    {
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public int HabitatId { get; set; }
        public DateTime DateSpotted { get; set; }
        public required string ObserverName { get; set; }
        public required string Notes { get; set; }

        public virtual required Animal Animal { get; set; }
        public virtual required Habitat Habitat { get; set; }
    }
}
