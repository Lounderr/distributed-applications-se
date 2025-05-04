using WildlifeTracker.Data.Models;
using WildlifeTracker.Services;

namespace WildlifeTracker.Controllers
{
    public class AnimalController(IGenericService<Animal> genericService) : GenericController<Animal>(genericService)
    {
    }
}
