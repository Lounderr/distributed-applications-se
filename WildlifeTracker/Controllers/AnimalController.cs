using WildlifeTracker.Data.Models;
using WildlifeTracker.Models.AnimalDtos;

namespace WildlifeTracker.Controllers
{
    public class AnimalController()
        : GenericController<Animal, CreateAnimalDto, ReadAnimalDto, UpdateAnimalDto>()
    {
    }
}
