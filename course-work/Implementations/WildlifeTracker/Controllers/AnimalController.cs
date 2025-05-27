using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

using WildlifeTracker.Data.Models;
using WildlifeTracker.Models.AnimalDtos;
using WildlifeTracker.Services.Data;

namespace WildlifeTracker.Controllers
{
    [SwaggerTag("Animal management operations")]
    public class AnimalController(IAnimalImageService animalImageService)
           : GenericController<Animal, CreateAnimalDto, ReadAnimalDto, UpdateAnimalDto>()
    {
        [HttpPut("{id}/image")]
        [SwaggerOperation(
            Summary = "Update animal image",
            Description = "Updates or creates an image for the specified animal",
            OperationId = "UpdateAnimalImage",
            Tags = new[] { "Animals" }
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Image successfully updated")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid image file or animal ID")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Animal not found")]
        public async Task<IActionResult> UpdateImage([FromRoute] int id, IFormFile file)
        {
            await animalImageService.CreateOrReplaceAsync(id, file);

            return this.NoContent();
        }

        [HttpDelete("{id}/image")]
        [SwaggerOperation(
            Summary = "Delete animal image",
            Description = "Deletes the image associated with the specified animal",
            OperationId = "DeleteAnimalImage",
            Tags = new[] { "Animals" }
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Image successfully deleted")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Animal or image not found")]
        public async Task<IActionResult> DeleteImage([FromRoute] int id)
        {
            await animalImageService.DeleteAsync(id);

            return this.NoContent();
        }
    }
}
