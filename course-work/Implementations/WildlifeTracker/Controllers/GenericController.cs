using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

using WildlifeTracker.Data.Models;
using WildlifeTracker.Data.Models.Interfaces;
using WildlifeTracker.Services.Data;

namespace WildlifeTracker.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [SwaggerTag("Generic CRUD operations for entities")]
    public abstract class GenericController<TEntity, TCreateDto, TReadDto, TUpdateDto>() : ControllerBase
        where TEntity : BaseEntity
        where TReadDto : IIdentifiable
    {
        private IGenericService<TEntity> genericService => this.HttpContext.RequestServices.GetRequiredService<IGenericService<TEntity>>();

        [HttpGet]
        [SwaggerOperation(
            Summary = "Get paginated list of entities",
            Description = "Retrieves a paginated list of entities with optional filtering, field selection, and sorting",
            OperationId = "GetAll",
            Tags = new[] { "Generic" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Successfully retrieved the list of entities")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request parameters")]
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
        [SwaggerOperation(
            Summary = "Get entity by ID",
            Description = "Retrieves a single entity by its unique identifier",
            OperationId = "GetById",
            Tags = new[] { "Generic" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Successfully retrieved the entity")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authenticated")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Entity not found")]
        public virtual async Task<IActionResult> GetById([FromRoute] int id)
        {
            var item = await this.genericService.GetByIdAsync<TReadDto>(id);
            return this.Ok(item);
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Create new entity",
            Description = "Creates a new entity with the provided data",
            OperationId = "Create",
            Tags = new[] { "Generic" }
        )]
        [SwaggerResponse(StatusCodes.Status201Created, "Entity successfully created")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid entity data provided")]
        public virtual async Task<IActionResult> Create([FromBody] TCreateDto item)
        {
            var createdItem = await this.genericService.AddAsync<TReadDto, TCreateDto>(item);
            return this.CreatedAtAction(nameof(GetById), new { createdItem.Id }, createdItem);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Update entity",
            Description = "Updates an existing entity with the provided data",
            OperationId = "Update",
            Tags = new[] { "Generic" }
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Entity successfully updated")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid entity data provided")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Entity not found")]
        public virtual async Task<IActionResult> Update([FromRoute] int id, [FromBody] TUpdateDto item)
        {
            await this.genericService.UpdateAsync(id, item);
            return this.NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Delete entity",
            Description = "Deletes an existing entity by its ID",
            OperationId = "Delete",
            Tags = new[] { "Generic" }
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Entity successfully deleted")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Entity not found")]
        public virtual async Task<IActionResult> Delete([FromRoute] int id)
        {
            await this.genericService.DeleteAsync(id);
            return this.NoContent();
        }
    }
}
