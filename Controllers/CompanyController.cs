using Microsoft.AspNetCore.Mvc;
using SWP391_Project.Helpers;
using SWP391_Project.Models;
using SWP391_Project.Services;
using SWP391_Project.ViewModels.Account;
using SWP391_Project.ViewModels.Company;

namespace SWP391_Project.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ICompanyService _companyService;
        private readonly ILocationService _locationService;
        private readonly IAccountService _accountService;

        public CompanyController(ICompanyService companyService, ILocationService locationService, IAccountService accountService)
        {
            _companyService = companyService;
            _locationService = locationService; 
            _accountService = accountService;
        }

        [RoleAuthorize(Role.COMPANY)]
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var userIdInt = int.Parse(userId);
            var company = await _companyService.GetCompanyByUserIdAsync(userIdInt);
            if (company == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Update session với thông tin mới nhất từ database
            HttpContext.Session.SetString("Name", company.Name);
            if (!string.IsNullOrEmpty(company.ImageUrl))
            {
                HttpContext.Session.SetString("ImageUrl", company.ImageUrl);
            }

            // Get company dashboard
            var viewModel = await _companyService.GetCompanyDashboardViewAsync(company.Id);

            return View(viewModel);
        }
        [RoleAuthorize(Role.COMPANY)]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userIdStr = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdStr);
            var model = await _companyService.GetProfileForEditAsync(userId);

            if (model == null) return RedirectToAction("Index");

            // Load lại danh sách Thành phố để đổ vào Dropdown
            ViewBag.Cities = await _locationService.GetCitiesAsync();

            return View(model);
        }
        [RoleAuthorize(Role.COMPANY)]
        [HttpPost]
        public async Task<IActionResult> Profile(CompanyProfileVM model)
        {
            var userIdStr = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");
            int userId = int.Parse(userIdStr);

            if (!ModelState.IsValid)
            {
                ViewBag.Cities = await _locationService.GetCitiesAsync();
                return View(model);
            }

            var result = await _companyService.UpdateProfileAsync(userId, model);

            if (result)
            {
                TempData["Success"] = "Cập nhật hồ sơ thành công!";
                // Cập nhật lại Session tên và ảnh mới (nếu có)
                var updatedCompany = await _companyService.GetCompanyByUserIdAsync(userId);
                if (updatedCompany != null)
                {
                    HttpContext.Session.SetString("Name", updatedCompany.Name);
                    if (!string.IsNullOrEmpty(updatedCompany.ImageUrl))
                        HttpContext.Session.SetString("ImageUrl", updatedCompany.ImageUrl);
                }
            }
            else
            {
                TempData["Error"] = "Có lỗi xảy ra, vui lòng thử lại.";
            }

            return RedirectToAction("Profile");
        }

        [RoleAuthorize(Role.COMPANY)]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            var userIdStr = HttpContext.Session.GetString("UserID");
            if(string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");
            int userId = int.Parse(userIdStr);

            if(!ModelState.IsValid)
            {
                var errors = string.Join("<br/>", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));

                TempData["Error"] = "Dữ liệu không hợp lệ:<br/>" + errors;
                return RedirectToAction("Profile");
            }

            var (success, error) = await _accountService.ChangePasswordAsync(userId, model.OldPassword, model.NewPassword);

            if (success)
            {
                TempData["Success"] = "Đổi mật khẩu thành công!";
            }
            else
            {
                TempData["Error"] = error;
            }

            return RedirectToAction("Profile");

        }
    }
}
