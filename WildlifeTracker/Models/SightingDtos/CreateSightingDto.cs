using System.ComponentModel.DataAnnotations;

using WildlifeTracker.Data.Models;
using WildlifeTracker.Helpers.DataAnotations;
using WildlifeTracker.Services.Mapping;

namespace WildlifeTracker.Models.SightingDtos
{
    public class CreateSightingDto : IMapTo<Sighting>
    {
        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        [StringLength(100)]
        public required string WeatherConditions { get; set; }

        [Required]
        [DateNotInFutureValidator]
        public DateTime SightingDateTime { get; set; }

        [Required]
        public int AnimalId { get; set; }

        [Required]
        public int HabitatId { get; set; }

        [Required]
        public required string ObserverId { get; set; }
    }
}
