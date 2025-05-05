using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

using WildlifeTracker.Constants;
using WildlifeTracker.Exceptions;

namespace WildlifeTracker.Services
{
    public class ImageProcessingService(IWebHostEnvironment env) : IImageProcessingService
    {
        public async Task SaveFormFileAsJpegAsync(IFormFile file, string outputFilePath)
        {
            if (file == null || file.Length == 0)
            {
                throw new ServiceException(ErrorCodes.ImageUploadFailed, "No file uploaded");
            }

            var contentType = file.ContentType;
            if (!contentType.StartsWith("image/"))
            {
                throw new ServiceException(ErrorCodes.ImageUploadFailed, "Invalid file type");
            }

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    using (var image = await Image.LoadAsync(stream))
                    {
                        var encoder = new JpegEncoder
                        {
                            Quality = 75
                        };

                        // Ensure the directory exists
                        string? directoryPath = Path.GetDirectoryName(env.WebRootPath + outputFilePath);

                        if (string.IsNullOrEmpty(directoryPath))
                        {
                            throw new DirectoryNotFoundException("Directory path is null or empty");
                        }

                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        await image.SaveAsync(env.WebRootPath + outputFilePath, encoder);
                    }
                }
            }
            catch (Exception)
            {
                throw new ServiceException(ErrorCodes.ImageUploadFailed, $"Error processing image");
            }
        }
    }
}
