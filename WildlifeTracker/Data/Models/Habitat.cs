namespace WildlifeTracker.Data.Models
{
    public class Habitat : BaseEntity
    {
        public required string Name { get; set; }
        public required string Location { get; set; }
        public double Size { get; set; }
        public required string Climate { get; set; }
        public double AverageTemperature { get; set; }
    }
}
