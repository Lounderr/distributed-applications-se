using WildlifeTracker.Data.Models;
using WildlifeTracker.Models.Sighting;
using WildlifeTracker.Services;

namespace WildlifeTracker.Controllers
{
    public class SightingController(IGenericService<Sighting> genericService)
        : GenericController<Sighting, CreateSightingDto, ReadSightingDto, UpdateSightingDto>(genericService)
    {
    }
}
