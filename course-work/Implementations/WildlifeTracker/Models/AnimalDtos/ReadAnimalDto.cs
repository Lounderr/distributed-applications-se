using WildlifeTracker.Data.Models;
using WildlifeTracker.Data.Models.Interfaces;

namespace WildlifeTracker.Models.AnimalDtos
{
    public class ReadAnimalDto : IMapFrom<Animal>, IIdentifiable
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Species { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public string? ImagePath { get; set; }
    }
}
