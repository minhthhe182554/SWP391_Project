using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dotenv.net;
using System.Net;

namespace SWP391_Project.Repositories.Storage
{
    public class CloudinaryStorageRepository : IStorageRepository
    {
        private Cloudinary? _cloudinaryClient;

        private Cloudinary GetClientInternal()
        {
            if (_cloudinaryClient != null) return _cloudinaryClient;

            // Load environment variables 
            DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));

            var cloudinaryUrl = Environment.GetEnvironmentVariable("CLOUDINARY_URL");
            if (string.IsNullOrEmpty(cloudinaryUrl))
            {
                throw new InvalidOperationException("CLOUDINARY_URL environment variable is not configured.");
            }
            var client = new Cloudinary(cloudinaryUrl);
            client.Api.Secure = true;
            _cloudinaryClient = client;
            return client;
        }

        public Cloudinary GetClient() => GetClientInternal();
        public string BuildImageUrl(string publicId)
        {
            if (string.IsNullOrEmpty(publicId)) throw new ArgumentNullException(nameof(publicId), "Public ID cannot be null or empty");
            var client = GetClientInternal();
            return client.Api.UrlImgUp.Format("png").BuildUrl(publicId);
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder, string? customPublicId = null)
        {
            if (file == null || file.Length == 0) throw new ArgumentException("File is empty or null");
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension)) throw new ArgumentException("Invalid file type. Only images are allowed.");
            if (file.Length > 5 * 1024 * 1024) throw new ArgumentException("File size must be less than 5MB");
            var cloudinary = GetClientInternal();
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                UseFilename = false,
                UniqueFilename = string.IsNullOrEmpty(customPublicId),
                Overwrite = !string.IsNullOrEmpty(customPublicId),
                Invalidate = true
            };
            if (!string.IsNullOrEmpty(customPublicId))
            {
                uploadParams.PublicId = customPublicId.StartsWith($"{folder}/", StringComparison.OrdinalIgnoreCase)
                    ? customPublicId.Substring(folder.Length + 1)
                    : customPublicId;
            }
            var result = await cloudinary.UploadAsync(uploadParams);
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Upload failed: {result.Error?.Message}");
            }
            return result.PublicId;
        }

        public async Task<(string PublicId, string SecureUrl)> UploadPdfAsync(IFormFile file, string folder, string? customPublicId = null)
        {
            if (file == null || file.Length == 0) throw new ArgumentException("File is empty or null");
            var fileExt = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (fileExt != ".pdf") throw new ArgumentException("Invalid file type. Only PDF files are allowed.");
            if (file.Length > 10 * 1024 * 1024) throw new ArgumentException("File size must be less than 10MB");
            var cloudinary = GetClientInternal();
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                UseFilename = false,
                UniqueFilename = string.IsNullOrEmpty(customPublicId),
                Overwrite = !string.IsNullOrEmpty(customPublicId),
                Invalidate = true
            };
            if (!string.IsNullOrEmpty(customPublicId))
            {
                uploadParams.PublicId = customPublicId.StartsWith($"{folder}/", StringComparison.OrdinalIgnoreCase)
                    ? customPublicId.Substring(folder.Length + 1)
                    : customPublicId;
            }
            var result = await cloudinary.UploadAsync(uploadParams);
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Upload failed: {result.Error?.Message}");
            }
            return (result.PublicId, result.SecureUrl?.ToString() ?? string.Empty);
        }

        public string BuildRawUrl(string publicId)
        {
            if (string.IsNullOrEmpty(publicId)) throw new ArgumentNullException(nameof(publicId), "Public ID cannot be null or empty");
            var client = GetClientInternal();
            return client.Api.UrlImgUp
                .Format("pdf")
                .BuildUrl(publicId);
        }

        public string BuildPdfImageUrl(string publicId, int page = 1, int width = 800, int density = 150)
        {
            if (string.IsNullOrEmpty(publicId)) throw new ArgumentNullException(nameof(publicId), "Public ID cannot be null or empty");
            var cloudinary = GetClientInternal();
            var transformation = new Transformation().Page(page).Density(density).Width(width).Crop("scale");
            var url = cloudinary.Api.UrlImgUp
                .ResourceType("image")
                .Format("jpg")
                .Transform(transformation)
                .BuildUrl(publicId);
            return url;
        }

        public async Task<bool> DeleteAssetAsync(string publicId, ResourceType resourceType = ResourceType.Raw)
        {
            if (string.IsNullOrEmpty(publicId)) return true;
            var cloudinary = GetClientInternal();
            var deletionParams = new DeletionParams(publicId) { ResourceType = resourceType };
            var result = await cloudinary.DestroyAsync(deletionParams);
            return result.StatusCode == HttpStatusCode.OK || result.Result == "not found";
        }
    }
}


