using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WildlifeTracker.Data.Models;
using WildlifeTracker.Data.Models.Interfaces;
using WildlifeTracker.Services.Data;

namespace WildlifeTracker.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class GenericController<TEntity, TCreateDto, TReadDto, TUpdateDto>() : ControllerBase
        where TEntity : BaseEntity
        where TReadDto : IIdentifiable
    {
        private IGenericService<TEntity> genericService => this.HttpContext.RequestServices.GetRequiredService<IGenericService<TEntity>>();

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

            var result = await this.genericService.GetFilteredAndPagedAsync<TReadDto>(page, size, filtersArr, fieldsArr, orderByArr);
            return this.Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize()]
        public virtual async Task<IActionResult> GetById([FromRoute] int id)
        {
            var item = await this.genericService.GetByIdAsync<TReadDto>(id);
            return this.Ok(item);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] TCreateDto item)
        {
            var createdItem = await this.genericService.AddAsync<TReadDto, TCreateDto>(item);
            return this.CreatedAtAction(nameof(GetById), new { createdItem.Id }, createdItem);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update([FromRoute] int id, [FromBody] TUpdateDto item)
        {
            await this.genericService.UpdateAsync(id, item);
            return this.NoContent();
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete([FromRoute] int id)
        {
            await this.genericService.DeleteAsync(id);
            return this.NoContent();
        }
    }
}
