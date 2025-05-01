namespace WildlifeTracker.Data.Models
{
    public class Sighting : BaseEntity
    {
        public int AnimalId { get; set; }
        public int HabitatId { get; set; }
        public required string WeatherConditions { get; set; } // Weather during the sighting  
        public required string Notes { get; set; }

        public virtual required Animal Animal { get; set; }
        public virtual required Habitat Habitat { get; set; }
    }
}
