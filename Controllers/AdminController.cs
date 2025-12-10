using Microsoft.AspNetCore.Mvc;
using SWP391_Project.Helpers;
using SWP391_Project.Models;
using SWP391_Project.Services;

namespace SWP391_Project.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [RoleAuthorize(Role.ADMIN)]
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var userIdInt = int.Parse(userId);
            var user = await _adminService.GetAdminUserByIdAsync(userIdInt);
            if (user != null)
            {
                HttpContext.Session.SetString("Name", user.Email);
            }

            // Get admin dashboard statistics
            var viewModel = await _adminService.GetAdminDashboardViewAsync();

            return View(viewModel);
        }
    }
}
