using WildlifeTracker.Data.Models;
using WildlifeTracker.Models.HabitatDtos;

namespace WildlifeTracker.Controllers
{
    public class HabitatController()
        : GenericController<Habitat, CreateHabitatDto, ReadHabitatDto, UpdateHabitatDto>()
    {
    }
}
