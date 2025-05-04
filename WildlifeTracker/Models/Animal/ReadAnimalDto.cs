namespace WildlifeTracker.Models.Animal
{
    public class ReadAnimalDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Species { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; }
        public int Height { get; set; }
    }
}
