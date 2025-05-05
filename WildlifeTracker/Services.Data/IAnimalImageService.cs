
namespace WildlifeTracker.Services.Data
{
    public interface IAnimalImageService
    {
        Task CreateOrReplaceAsync(int animalId, IFormFile file);
        Task DeleteAsync(int id);
    }
}