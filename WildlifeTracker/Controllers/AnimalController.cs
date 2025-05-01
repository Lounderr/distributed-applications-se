using WildlifeTracker.Data.Models;
using WildlifeTracker.Data.Repositories;

namespace WildlifeTracker.Controllers
{
    public class AnimalController : GenericController<Animal>
    {
        public AnimalController(IRepository<Animal> repository) : base(repository)
        {
        }
    }
}
