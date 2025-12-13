using Microsoft.AspNetCore.Mvc;
using SWP391_Project.Helpers;
using SWP391_Project.Models;
using SWP391_Project.Services;
using SWP391_Project.ViewModels.Account;
using SWP391_Project.ViewModels.Company;
using Microsoft.AspNetCore.Authorization;

namespace SWP391_Project.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ICompanyService _companyService;
        private readonly ILocationService _locationService;
        private readonly IAccountService _accountService;
        private readonly IConfiguration _configuration;

        public CompanyController(ICompanyService companyService, ILocationService locationService, IAccountService accountService, IConfiguration configuration)
        {
            _companyService = companyService;
            _locationService = locationService; 
            _accountService = accountService;
            _configuration = configuration;
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
            ViewBag.GoogleMapsApiKey = _configuration[AppConstants.ConfigurationKeys.GoogleMapsApiKey];

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
                ViewBag.GoogleMapsApiKey = _configuration[AppConstants.ConfigurationKeys.GoogleMapsApiKey];
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

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Detail(int id)
        {
            var vm = await _companyService.GetCompanyDetailAsync(id);
            if (vm == null) return NotFound();
            ViewBag.GoogleMapsApiKey = _configuration[AppConstants.ConfigurationKeys.GoogleMapsApiKey];
            return View(vm);
        }

        [RoleAuthorize(Role.COMPANY)]
        [HttpPost]
        public async Task<IActionResult> UpdateBasicProfile(CompanyProfileVM model)
        {
            var userIdStr = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");
            int userId = int.Parse(userIdStr);

            // Bỏ qua validate các trường không thuộc basic
            ModelState.Remove(nameof(CompanyProfileVM.City));
            ModelState.Remove(nameof(CompanyProfileVM.Ward));
            ModelState.Remove(nameof(CompanyProfileVM.Address));
            ModelState.Remove(nameof(CompanyProfileVM.Latitude));
            ModelState.Remove(nameof(CompanyProfileVM.Longitude));

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại.";
                return RedirectToAction("Profile");
            }

            var result = await _companyService.UpdateBasicProfileAsync(userId, model);
            if (result)
            {
                TempData["Success"] = "Cập nhật thông tin công ty thành công!";
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
                TempData["Error"] = "Có lỗi xảy ra khi lưu thông tin công ty.";
            }

            return RedirectToAction("Profile");
        }

        [RoleAuthorize(Role.COMPANY)]
        [HttpPost]
        public async Task<IActionResult> UpdateAddressProfile(CompanyProfileVM model)
        {
            var userIdStr = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");
            int userId = int.Parse(userIdStr);

            // Bỏ qua validate các trường không thuộc address
            ModelState.Remove(nameof(CompanyProfileVM.Name));
            ModelState.Remove(nameof(CompanyProfileVM.PhoneNumber));
            ModelState.Remove(nameof(CompanyProfileVM.Website));
            ModelState.Remove(nameof(CompanyProfileVM.Description));
            ModelState.Remove(nameof(CompanyProfileVM.AvatarFile));

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu địa chỉ không hợp lệ, vui lòng kiểm tra lại.";
                return RedirectToAction("Profile");
            }

            var result = await _companyService.UpdateAddressProfileAsync(userId, model);
            if (result)
            {
                TempData["Success"] = "Cập nhật địa chỉ & tọa độ thành công!";
            }
            else
            {
                TempData["Error"] = "Có lỗi xảy ra khi lưu địa chỉ.";
            }

            return RedirectToAction("Profile");
        }
    }
}
