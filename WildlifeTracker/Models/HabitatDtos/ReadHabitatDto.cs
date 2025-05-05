using WildlifeTracker.Data.Models;

namespace WildlifeTracker.Models.HabitatDtos
{
    public class ReadHabitatDto : IMapFrom<Habitat>
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Location { get; set; }
        public double Size { get; set; }
        public required string Climate { get; set; }
        public double AverageTemperature { get; set; }
    }
}
