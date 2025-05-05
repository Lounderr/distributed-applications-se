using WildlifeTracker.Data.Models;
using WildlifeTracker.Models.SightingDtos;

namespace WildlifeTracker.Controllers
{
    public class SightingController()
        : GenericController<Sighting, CreateSightingDto, ReadSightingDto, UpdateSightingDto>()
    {
    }
}
