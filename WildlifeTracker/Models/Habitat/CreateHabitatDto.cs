using System.ComponentModel.DataAnnotations;

namespace WildlifeTracker.Models.Habitat
{
    public class CreateHabitatDto
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
