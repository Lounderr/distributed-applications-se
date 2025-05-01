namespace WildlifeTracker.Data.Models
{
    public class Animal
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Species { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
