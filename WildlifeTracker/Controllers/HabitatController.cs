using WildlifeTracker.Data.Models;
using WildlifeTracker.Models.Habitat;
using WildlifeTracker.Services;

namespace WildlifeTracker.Controllers
{
    public class HabitatController(IGenericService<Habitat> genericService)
        : GenericController<Habitat, CreateHabitatDto, ReadHabitatDto, UpdateHabitatDto>(genericService)
    {
    }
}
