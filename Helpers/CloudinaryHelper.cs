using CloudinaryDotNet;
using dotenv.net;

namespace SWP391_Project.Helpers
{
    public interface ICloudinaryHelper
    {
        // Initializes and returns a Cloudinary client instance
        Cloudinary GetCloudinaryClient();

        // Builds a secure image URL for a given public ID
        string BuildImageUrl(string publicId);
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
    }
}
