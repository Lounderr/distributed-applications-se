using Microsoft.AspNetCore.Mvc;

using WildlifeTracker.Data;

using YourNamespace.Data.Models;

namespace WildlifeTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HabitatController : GenericController<Habitat>
    {
        public HabitatController(ApplicationDbContext context) : base(context)
        {
        }
    }
}
