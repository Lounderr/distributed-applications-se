using WildlifeTracker.Data.Models;
using WildlifeTracker.Models.Animal;
using WildlifeTracker.Services;

namespace WildlifeTracker.Controllers
{
    public class AnimalController(IGenericService<Animal> genericService)
        : GenericController<Animal, CreateAnimalDto, ReadAnimalDto, UpdateAnimalDto>(genericService)
    {
    }
}
