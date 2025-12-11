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
        Task<string> UploadImageAsync(IFormFile file, string folder, string customPublicId = null);
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

                // Build secure image URL using UrlImgUp method
                var url = cloudinary.Api.UrlImgUp.Format("png").BuildUrl(publicId);

                return url;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder, string customPublicId = null)
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

                // Set custom public ID if provided
                if (!string.IsNullOrEmpty(customPublicId))
                {
                    uploadParams.PublicId = $"{folder}/{customPublicId}";
                }

                // Upload image
                var uploadResult = await cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
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
    }
}
