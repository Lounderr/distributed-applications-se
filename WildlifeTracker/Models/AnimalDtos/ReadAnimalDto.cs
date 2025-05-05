namespace WildlifeTracker.Models.AnimalDtos
{
    public class ReadAnimalDto : IMapFrom<Data.Models.Animal>
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Species { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
    }
}
