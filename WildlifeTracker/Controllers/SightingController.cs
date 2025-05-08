using Microsoft.AspNetCore.Mvc;

using WildlifeTracker.Data.Models;
using WildlifeTracker.Helpers.Extensions;
using WildlifeTracker.Models.SightingDtos;

namespace WildlifeTracker.Controllers
{
    public class SightingController()
        : GenericController<Sighting, CreateSightingDto, ReadSightingDto, UpdateSightingDto>()
    {
        public override Task<IActionResult> Create([FromBody] CreateSightingDto item)
        {
            item.ObserverId = this.User.GetUserId();

            return base.Create(item);
        }

        public override Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateSightingDto item)
        {
            item.ObserverId = this.User.GetUserId();

            return base.Update(id, item);
        }
    }
}
