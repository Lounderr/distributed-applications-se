namespace YourNamespace.Data.Models
{
    public class Sighting
    {
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public int HabitatId { get; set; }
        public DateTime DateSpotted { get; set; }
        public string ObserverName { get; set; }
        public string Notes { get; set; }

        public Animal Animal { get; set; }
        public Habitat Habitat { get; set; }
    }
}
