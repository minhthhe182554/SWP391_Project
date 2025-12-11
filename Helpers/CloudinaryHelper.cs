using System.Net;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dotenv.net;
using Microsoft.AspNetCore.Http;

namespace SWP391_Project.Helpers
{
    public interface ICloudinaryHelper
    {
        // Initializes and returns a Cloudinary client instance
        Cloudinary GetCloudinaryClient();

        // Builds a secure image URL for a given public ID
        string BuildImageUrl(string publicId);

        // Uploads an image to Cloudinary and returns the public ID
        Task<string> UploadImageAsync(IFormFile file, string folder, string? customPublicId = null);

        // Uploads a PDF/raw file to Cloudinary and returns public ID + secure URL
        Task<(string PublicId, string SecureUrl)> UploadPdfAsync(IFormFile file, string folder, string? customPublicId = null);

        // Builds a secure raw URL (for PDFs) from public ID
        string BuildRawUrl(string publicId);

        // Builds an image URL for a given PDF public ID and page
        string BuildPdfImageUrl(string publicId, int page = 1, int width = 800, int density = 150);

        // Deletes a file (raw or image) by public ID
        Task<bool> DeleteAssetAsync(string publicId, ResourceType resourceType = ResourceType.Raw);
    }

    public class CloudinaryHelper : ICloudinaryHelper
    {
        public CloudinaryHelper() {}

        public Cloudinary GetCloudinaryClient()
        {
            try
            {
                // Load environment variables from .env file
                DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));

                // Get CLOUDINARY_URL from environment
                var cloudinaryUrl = Environment.GetEnvironmentVariable("CLOUDINARY_URL");

                if (string.IsNullOrEmpty(cloudinaryUrl))
                {
                    throw new InvalidOperationException("CLOUDINARY_URL environment variable is not configured.");
                }

                // Initialize Cloudinary client
                var cloudinary = new Cloudinary(cloudinaryUrl);
                cloudinary.Api.Secure = true;

                return cloudinary;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string BuildImageUrl(string publicId)
        {
            try
            {
                if (string.IsNullOrEmpty(publicId))
                {
                    throw new ArgumentNullException(nameof(publicId), "Public ID cannot be null or empty");
                }

                var cloudinary = GetCloudinaryClient();
                var url = cloudinary.Api.UrlImgUp.Format("png").BuildUrl(publicId);

                return url;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder, string? customPublicId = null)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("File is empty or null");
                }

                // Validate file is an image
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new ArgumentException("Invalid file type. Only images are allowed.");
                }

                // Validate file size (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    throw new ArgumentException("File size must be less than 5MB");
                }

                var cloudinary = GetCloudinaryClient();

                // Create upload parameters
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folder,
                    UseFilename = false,
                    UniqueFilename = string.IsNullOrEmpty(customPublicId), // Only unique if no custom ID
                    Overwrite = !string.IsNullOrEmpty(customPublicId), // Overwrite if custom ID provided
                    Invalidate = true // Clear CDN cache when overwriting
                };

                // Set custom public ID if provided (folder is already set above, don't prepend again)
                if (!string.IsNullOrEmpty(customPublicId))
                {
                    // Remove folder prefix if already present to avoid double folder
                    uploadParams.PublicId = customPublicId.StartsWith($"{folder}/", StringComparison.OrdinalIgnoreCase)
                        ? customPublicId.Substring(folder.Length + 1)
                        : customPublicId;
                }

                // Upload image
                var uploadResult = await cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Upload failed: {uploadResult.Error?.Message}");
                }

                // Return public ID
                return uploadResult.PublicId;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(string PublicId, string SecureUrl)> UploadPdfAsync(IFormFile file, string folder, string? customPublicId = null)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("File is empty or null");
                }

                // Validate file is PDF
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (fileExtension != ".pdf")
                {
                    throw new ArgumentException("Invalid file type. Only PDF files are allowed.");
                }

                // Validate file size (max 10MB)
                if (file.Length > 10 * 1024 * 1024)
                {
                    throw new ArgumentException("File size must be less than 10MB");
                }

                var cloudinary = GetCloudinaryClient();

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

                // Set custom public ID if provided (folder is already set above, don't prepend again)
                if (!string.IsNullOrEmpty(customPublicId))
                {
                    // Remove folder prefix if already present to avoid double folder
                    uploadParams.PublicId = customPublicId.StartsWith($"{folder}/", StringComparison.OrdinalIgnoreCase)
                        ? customPublicId.Substring(folder.Length + 1)
                        : customPublicId;
                }

                var uploadResult = await cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Upload failed: {uploadResult.Error?.Message}");
                }

                return (uploadResult.PublicId, uploadResult.SecureUrl?.ToString() ?? string.Empty);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string BuildRawUrl(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
            {
                throw new ArgumentNullException(nameof(publicId), "Public ID cannot be null or empty");
            }

            var cloudinary = GetCloudinaryClient();
            // deliver original pdf (even though stored as image-type)
            return cloudinary.Api.UrlImgUp
                .Format("pdf")
                .BuildUrl(publicId);
        }

        public string BuildPdfImageUrl(string publicId, int page = 1, int width = 800, int density = 150)
        {
            if (string.IsNullOrEmpty(publicId))
            {
                throw new ArgumentNullException(nameof(publicId), "Public ID cannot be null or empty");
            }

            var cloudinary = GetCloudinaryClient();
            var transformation = new Transformation()
                .Page(page)
                .Density(density)
                .Width(width)
                .Crop("scale");

            // Build URL; add format jpg to render as image
            var url = cloudinary.Api.UrlImgUp
                .ResourceType("image")
                .Format("jpg")
                .Transform(transformation)
                .BuildUrl(publicId);

            return url;
        }

        public async Task<bool> DeleteAssetAsync(string publicId, ResourceType resourceType = ResourceType.Raw)
        {
            if (string.IsNullOrEmpty(publicId))
            {
                return true;
            }

            var cloudinary = GetCloudinaryClient();
            var deletionParams = new DeletionParams(publicId)
            {
                ResourceType = resourceType
            };

            var result = await cloudinary.DestroyAsync(deletionParams);
            return result.StatusCode == HttpStatusCode.OK || result.Result == "not found";
        }
    }
}
