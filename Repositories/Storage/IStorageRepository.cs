using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace SWP391_Project.Repositories.Storage
{
    public interface IStorageRepository
    {
        Cloudinary GetClient();
        string BuildImageUrl(string publicId);
        Task<string> UploadImageAsync(IFormFile file, string folder, string? customPublicId = null);
        Task<(string PublicId, string SecureUrl)> UploadPdfAsync(IFormFile file, string folder, string? customPublicId = null);
        string BuildRawUrl(string publicId);
        string BuildPdfImageUrl(string publicId, int page = 1, int width = 800, int density = 150);
        Task<bool> DeleteAssetAsync(string publicId, ResourceType resourceType = ResourceType.Raw);
    }
}


