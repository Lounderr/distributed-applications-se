using WildlifeTracker.Data.Models;
using WildlifeTracker.Data.Repositories;

namespace WildlifeTracker.Controllers
{
    public class SightingController : GenericController<Sighting>
    {
        public SightingController(IRepository<Sighting> repository) : base(repository)
        {
        }
    }
}
