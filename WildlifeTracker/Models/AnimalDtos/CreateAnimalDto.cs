using System.ComponentModel.DataAnnotations;

using WildlifeTracker.Data.Models;
using WildlifeTracker.Services.Mapping;

namespace WildlifeTracker.Models.AnimalDtos
{
    // Animal DTOs
    public class CreateAnimalDto : IMapTo<Animal>
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Species { get; set; } = null!;

        [Required]
        [Range(0, int.MaxValue)]
        public int Age { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double Weight { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double Height { get; set; }
    }
}
