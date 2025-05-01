using WildlifeTracker.Data.Models;
using WildlifeTracker.Data.Repositories;

namespace WildlifeTracker.Controllers
{
    public class HabitatController : GenericController<Habitat>
    {
        public HabitatController(IRepository<Habitat> repository) : base(repository)
        {
        }
    }
}
