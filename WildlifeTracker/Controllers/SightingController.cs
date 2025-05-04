using WildlifeTracker.Data.Models;
using WildlifeTracker.Services;

namespace WildlifeTracker.Controllers
{
    public class SightingController(IGenericService<Sighting> genericService) : GenericController<Sighting>(genericService)
    {
    }
}
