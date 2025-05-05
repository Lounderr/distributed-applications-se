using Microsoft.AspNetCore.Mvc;

using WildlifeTracker.Data.Models;
using WildlifeTracker.Models.AnimalDtos;
using WildlifeTracker.Services.Data;

namespace WildlifeTracker.Controllers
{
    public class AnimalController(IAnimalImageService animalImageService)
           : GenericController<Animal, CreateAnimalDto, ReadAnimalDto, UpdateAnimalDto>()
    {
        [HttpPut("{id}/image")]
        public async Task<IActionResult> UpdateImage([FromRoute] int id, IFormFile file)
        {
            await animalImageService.CreateOrReplaceAsync(id, file);

            return this.NoContent();
        }

        [HttpDelete("{id}/image")]
        public async Task<IActionResult> DeleteImage([FromRoute] int id)
        {
            await animalImageService.DeleteAsync(id);

            return this.NoContent();
        }
    }
}
