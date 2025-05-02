using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WildlifeTracker.Constants;
using WildlifeTracker.Data.Models.Interfaces;
using WildlifeTracker.Data.Repositories;
using WildlifeTracker.Exceptions;

namespace WildlifeTracker.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class GenericController<T>(IRepository<T> repository) : ControllerBase
        where T : class, IIdentifiable
    {
        [HttpGet]
        public virtual async Task<IActionResult> GetAll([FromQuery] int page, [FromQuery] int size, [FromQuery] string? filters, [FromQuery] string? fields)
        {
            // TODO: Move data processing into services and clean up repositories
            // TODO: Add DTOs
            // TODO: Add input validation and store phone numbers
            // TODO: Add user list GET (last login / refresh

            if (size == 0)
                size = 15;

            string[]? filtersArr = filters?.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            string[]? fieldsArr = fields?.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            try
            {
                return this.Ok(await repository.SearchAsync(page, size, filtersArr, fieldsArr));
            }
            catch (ArgumentException ex)
            {
                throw new BusinessException(ErrorCodes.SearchParamsInvalid, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById(int id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
                throw new NotFoundException($"Entity {typeof(T).Name} with id {id} not found");

            return this.Ok(item);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] T item)
        {
            await repository.AddAsync(item);
            return this.CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(int id, [FromBody] T item)
        {
            if (id != item.Id)
                throw new BusinessException(ErrorCodes.IdMismatch, "The 'id' in the URL does not match the 'Id' of the entity");

            var existingItem = await repository.GetByIdAsync(id);
            if (existingItem == null)
                throw new NotFoundException($"Entity {typeof(T).Name} with id {id} not found");

            await repository.UpdateAsync(item);
            return this.NoContent();
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            await repository.DeleteAsync(id);
            return this.NoContent();
        }
    }
}
