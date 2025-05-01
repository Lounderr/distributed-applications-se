using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WildlifeTracker.Data.Repositories;

namespace WildlifeTracker.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class GenericController<T> : ControllerBase where T : class
    {
        private readonly IRepository<T> _repository;

        public GenericController(IRepository<T> repository)
        {
            this._repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Dictionary<string, string> queryParams)
        {
            // TODO: Add functionality that allows the user to choose what the API should return. (e.g. /api/v1/animal?fields=species,age)

            IEnumerable<T> results;
            if (!queryParams.Any())
                results = await this._repository.GetAllAsNoTrackingAsync();
            else
                results = await this._repository.SearchAsync(queryParams);
            return this.Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await this._repository.GetByIdAsync(id);
            if (item == null)
                return this.NotFound();

            return this.Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] T item)
        {
            if (item == null)
                return this.BadRequest();

            await this._repository.AddAsync(item);
            return this.CreatedAtAction(nameof(GetById), new { id = item }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] T item)
        {
            if (item == null)
                return this.BadRequest();

            await this._repository.UpdateAsync(item);
            return this.NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await this._repository.DeleteAsync(id);
            return this.NoContent();
        }
    }
}
