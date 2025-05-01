using Microsoft.AspNetCore.Mvc;

using WildlifeTracker.Data;

using YourNamespace.Data.Models;

namespace WildlifeTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnimalController : GenericController<Animal>
    {
        public AnimalController(ApplicationDbContext context) : base(context)
        {
        }
    }
}
