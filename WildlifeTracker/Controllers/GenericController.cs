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
        public virtual async Task<IActionResult> GetAll([FromQuery] int page, [FromQuery] int size, [FromQuery] Dictionary<string, string> queryParams)
        {
            // TODO: Add functionality that allows the user to choose what the API should return. (e.g. /api/v1/animal?fields=species,age)
            // TODO: Add pagination functionality (e.g. /api/v1/animal?page=1&size=10)

            IEnumerable<T> results;

            try
            {
                results = await repository.SearchAsync(page, size, queryParams.Skip(2).ToDictionary());
            }
            catch (ArgumentException ex)
            {
                throw new CustomValidationException(ErrorCodes.SearchParamsInvalid, ex.Message);
            }
            return this.Ok(results);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById(int id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
                return this.NotFound();

            return this.Ok(item);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] T item)
        {
            if (item == null)
                return this.BadRequest();

            await repository.AddAsync(item);
            return this.CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(int id, [FromBody] T item)
        {
            if (item == null)
                return this.BadRequest();

            if (id != item.Id)
                return this.BadRequest("The 'id' in the URL does not match the 'Id' of the entity.");

            var existingItem = await repository.GetByIdAsync(id);
            if (existingItem == null)
                return this.NotFound();

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
