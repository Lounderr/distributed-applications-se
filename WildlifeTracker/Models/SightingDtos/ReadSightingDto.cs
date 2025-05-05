using WildlifeTracker.Data.Models;

namespace WildlifeTracker.Models.SightingDtos
{
    public class ReadSightingDto : IMapFrom<Sighting>
    {
        public int Id { get; set; }
        public string? Notes { get; set; }
        public required string WeatherConditions { get; set; }
        public DateTime SightingDateTime { get; set; }
        public int AnimalId { get; set; }
        public int HabitatId { get; set; }
        public required string ObserverId { get; set; }
    }
}
