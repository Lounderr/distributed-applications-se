using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WildlifeTracker.Data.Models.Interfaces;
using WildlifeTracker.Services;

namespace WildlifeTracker.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class GenericController<T>(IGenericService<T> service) : ControllerBase
       where T : class, IIdentifiable
    {
        [HttpGet]
        public virtual async Task<IActionResult> GetAll(
            [FromQuery] int page,
            [FromQuery] int size,
            [FromQuery] string? filters,
            [FromQuery] string? fields,
            [FromQuery] string? orderBy)
        {
            if (size == 0)
                size = 15;

            string[]? filtersArr = filters?.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            string[]? fieldsArr = fields?.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            string[]? orderByArr = orderBy?.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);


            var result = await service.GetFilteredAndPagedAsync(page, size, filtersArr, fieldsArr, orderByArr);
            return this.Ok(result);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById(int id)
        {
            var item = await service.GetByIdAsync(id);

            return this.Ok(item);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] T item)
        {
            await service.AddAsync(item);
            return this.CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(int id, [FromBody] T item)
        {
            await service.UpdateAsync(id, item);
            return this.NoContent();
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            await service.DeleteAsync(id);
            return this.NoContent();
        }
    }
}
