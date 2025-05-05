using System.ComponentModel.DataAnnotations;

using WildlifeTracker.Data.Models;
using WildlifeTracker.Services.Mapping;

namespace WildlifeTracker.Models.HabitatDtos
{
    public class CreateHabitatDto : IMapTo<Habitat>
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Location { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public double Size { get; set; }

        [Required]
        [StringLength(100)]
        public string Climate { get; set; } = null!;

        [Range(double.MinValue, double.MaxValue)]
        public double AverageTemperature { get; set; }
    }
}
