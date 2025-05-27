using WildlifeTracker.Constants;
using WildlifeTracker.Data.Models;
using WildlifeTracker.Data.Repositories;
using WildlifeTracker.Exceptions;

namespace WildlifeTracker.Services.Data
{
    public class AnimalImageService(
            IImageProcessingService imageProcessing,
            IDeletableEntityRepository<Animal> repo,
            IResourceAccessService resourceAccess,
            ICurrentUserService user
        ) : IAnimalImageService
    {
        public async Task CreateOrReplaceAsync(int animalId, IFormFile file)
        {
            var animalEntity = await repo.GetByIdAsync(animalId);

            if (animalEntity == null)
            {
                throw new NotFoundException($"Animal with ID {animalId} not found");
            }

            resourceAccess.Authorize(animalEntity);

            if (file == null || file.Length == 0)
                throw new ServiceException(ErrorCodes.ImageUploadFailed, "No file uploaded");

            if (file.Length > 5 * 1024 * 1024) // 5 MB   
                throw new ServiceException(ErrorCodes.ImageUploadFailed, "File size exceeds the limit of 5 MB");

            var fileName = animalId.ToString() + ".jpeg";
            var savePath = Path.Combine("animals-images", fileName);

            await imageProcessing.SaveFormFileAsJpegAsync(file, savePath);

            animalEntity.ImagePath = savePath;

            await repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var animalEntity = await repo.GetByIdAsync(id);
            if (animalEntity == null)
            {
                throw new NotFoundException($"Animal with ID {id} not found");
            }

            resourceAccess.Authorize(animalEntity);

            if (animalEntity.ImagePath != null)
            {
                var filePath = Path.Combine("animals-images", animalEntity.ImagePath);
                File.Delete(filePath);
            }

            animalEntity.ImagePath = null;
            await repo.SaveChangesAsync();
        }
    }
}
