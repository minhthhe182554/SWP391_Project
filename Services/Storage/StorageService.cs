using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using SWP391_Project.Repositories.Storage;
using CloudinaryDotNet.Actions;

namespace SWP391_Project.Services.Storage
{
    public class StorageService : IStorageService
    {
        private readonly IStorageRepository _repository;

        public StorageService(IStorageRepository repository)
        {
            _repository = repository;
        }

        public string BuildImageUrl(string publicId) => _repository.BuildImageUrl(publicId);
        public Task<string> UploadImageAsync(IFormFile file, string folder, string? customPublicId = null)
            => _repository.UploadImageAsync(file, folder, customPublicId);
        public Task<(string PublicId, string SecureUrl)> UploadPdfAsync(IFormFile file, string folder, string? customPublicId = null)
            => _repository.UploadPdfAsync(file, folder, customPublicId);
        public string BuildRawUrl(string publicId) => _repository.BuildRawUrl(publicId);
        public string BuildPdfImageUrl(string publicId, int page = 1, int width = 800, int density = 150)
            => _repository.BuildPdfImageUrl(publicId, page, width, density);
        public Task<bool> DeleteAssetAsync(string publicId, ResourceType resourceType = ResourceType.Raw)
            => _repository.DeleteAssetAsync(publicId, resourceType);
    }
}


