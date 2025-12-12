using Microsoft.AspNetCore.Mvc;
using SWP391_Project.Services.Storage;
using SWP391_Project.ViewModels.Shared;

namespace SWP391_Project.ViewComponents
{
    public class UserDropdownViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStorageService _storageService;

        public UserDropdownViewComponent(IHttpContextAccessor httpContextAccessor, IStorageService storageService)
        {
            _httpContextAccessor = httpContextAccessor;
            _storageService = storageService;
        }

        public IViewComponentResult Invoke()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var session = httpContext?.Session;

            var email = session?.GetString("Email") ?? string.Empty;
            var userId = session?.GetString("UserID") ?? "N/A";
            var name = session?.GetString("Name") ?? "User";
            var role = session?.GetString("Role") ?? "CANDIDATE";
            var storedImage = session?.GetString("ImageUrl");

            var defaultAvatar = "/imgs/ic_default_avatar.png";
            var avatarUrl = defaultAvatar;

            if (!string.IsNullOrEmpty(storedImage))
            {
                if (storedImage.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    avatarUrl = storedImage;
                }
                else
                {
                    try
                    {
                        avatarUrl = _storageService.BuildImageUrl(storedImage);
                    }
                    catch
                    {
                        avatarUrl = defaultAvatar;
                    }
                }
            }

            var vm = new UserDropdownVM
            {
                UserId = userId,
                Email = email,
                Name = name,
                Role = role,
                AvatarUrl = avatarUrl
            };

            return View(vm);
        }
    }
}


