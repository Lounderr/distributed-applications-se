using WildlifeTracker.Data.Models;
using WildlifeTracker.Data.Models.Interfaces;

namespace WildlifeTracker.Models.HabitatDtos
{
    public class ReadHabitatDto : IMapFrom<Habitat>, IIdentifiable
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Location { get; set; }
        public double Size { get; set; }
        public required string Climate { get; set; }
        public double AverageTemperature { get; set; }
    }
}
