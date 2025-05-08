using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Mvc.ModelBinding;

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

        [BindNever]
        [JsonIgnore] // TODO: Take a second look at this to confirm that it is not bad practice
        public string? ObserverId { get; set; }
    }
}
