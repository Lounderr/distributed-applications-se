using WildlifeTracker.Data.Models;
using WildlifeTracker.Data.Models.Interfaces;

namespace WildlifeTracker.Models.SightingDtos
{
    public class ReadSightingDto : IMapFrom<Sighting>, IIdentifiable
    {
        public int Id { get; set; }
        public string? Notes { get; set; }
        public required string WeatherConditions { get; set; }
        public DateTime SightingDateTime { get; set; }
        public int AnimalId { get; set; }
        public int HabitatId { get; set; }

        // TODO: Idea - materialize the db query very late, so you can use IHaveCustomMappings to do DB operations
        public required string ObserverId { get; set; }
    }
}
