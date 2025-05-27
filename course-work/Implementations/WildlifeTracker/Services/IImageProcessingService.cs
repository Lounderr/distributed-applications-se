namespace WildlifeTracker.Services
{
    public interface IImageProcessingService
    {
        Task SaveFormFileAsJpegAsync(IFormFile file, string outputFilePath);
    }
}