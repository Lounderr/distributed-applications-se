using Microsoft.AspNetCore.Mvc;

using WildlifeTracker.Data;

using YourNamespace.Data.Models;

namespace WildlifeTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SightingController : GenericController<Sighting>
    {
        public SightingController(ApplicationDbContext context) : base(context)
        {
        }
    }
}
