using WildlifeTracker.Data.Models;
using WildlifeTracker.Services;

namespace WildlifeTracker.Controllers
{
    public class HabitatController(IGenericService<Habitat> genericService) : GenericController<Habitat>(genericService)
    {
    }
}
