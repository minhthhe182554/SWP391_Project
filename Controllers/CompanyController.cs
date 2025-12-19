using Microsoft.AspNetCore.Mvc;
using SWP391_Project.Helpers;
using SWP391_Project.Models.Enums;
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
        private readonly IJobService _jobService;
        private readonly IApplicationService _applicationService;

        public CompanyController(ICompanyService companyService, ILocationService locationService, IAccountService accountService, IConfiguration configuration, IJobService jobService, IApplicationService applicationService)
        {
            _companyService = companyService;
            _locationService = locationService;
            _accountService = accountService;
            _configuration = configuration;
            _jobService = jobService;
            _applicationService = applicationService;
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

            HttpContext.Session.SetString("Name", company.Name);
            if (!string.IsNullOrEmpty(company.ImageUrl))
            {
                HttpContext.Session.SetString("ImageUrl", company.ImageUrl);
            }

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
        [RoleAuthorize(Role.COMPANY)]
        [HttpGet]
        public async Task<IActionResult> PostJob()
        {
            var userIdStr = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

            var model = await _jobService.GetPostJobModelAsync();

            model.Cities = await _locationService.GetCitiesAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetWards(string cityCode)
        {
            if (string.IsNullOrEmpty(cityCode)) return Json(new List<object>());

            var wards = await _locationService.GetWardsByCityCodeAsync(cityCode);
            return Json(wards);
        }

        [RoleAuthorize(Role.COMPANY)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostJob(PostJobVM model)
        {
            var userIdStr = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                try
                {
                    await _jobService.AddJobAsync(int.Parse(userIdStr), model);

                    TempData["Success"] = "Đăng tin tuyển dụng thành công!";
                    return RedirectToAction("Index", "Company"); 
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
                }
            }

            var dropdownData = await _jobService.GetPostJobModelAsync();
            model.Locations = dropdownData.Locations;
            model.Skills = dropdownData.Skills;
            model.Domains = dropdownData.Domains;

            return View(model);
        }
        [RoleAuthorize(Role.COMPANY)]
        [HttpGet]
        public async Task<IActionResult> ManageJobs(string status = "all")
        {
            var userIdStr = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

            ViewBag.CurrentStatus = status;

            var list = await _jobService.GetCompanyJobsAsync(int.Parse(userIdStr), status);
            return View(list);
        }

        [RoleAuthorize(Role.COMPANY)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RepostJob(int id)
        {
            var userIdStr = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

            try
            {
                await _jobService.RepostJobAsync(int.Parse(userIdStr), id);
                TempData["Success"] = "Đăng lại tin thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
            }

            return RedirectToAction("ManageJobs");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StopRecruitment(int id)
        {
            var userIdStr = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

            try
            {
                await _jobService.StopRecruitmentAsync(int.Parse(userIdStr), id);
                TempData["Success"] = "Đã dừng tuyển dụng tin này.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
            }

            return RedirectToAction("ManageJobs");
        }

        [HttpGet]
        public async Task<IActionResult> EditJob(int id)
        {
            bool canEdit = await _jobService.CanEditJobAsync(id);

            if (!canEdit)
            {
                TempData["Error"] = "Không thể sửa tin tuyển dụng khi đã có ứng viên nộp hồ sơ!";
                return RedirectToAction("ManageJobs");
            }

            return Content("Form sửa tin sẽ hiện ở đây"); 
        }

        [RoleAuthorize(Role.COMPANY)]
        [HttpGet]
        public async Task<IActionResult> JobApplicants(int id)
        {
            var userIdStr = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");
            int userId = int.Parse(userIdStr);

            try
            {
                var company = await _companyService.GetCompanyByUserIdAsync(userId);
                if (company == null)
                {
                    TempData["Error"] = "Vui lòng cập nhật hồ sơ công ty trước.";
                    return RedirectToAction("Profile");
                }

                var vm = await _applicationService.GetApplicantsForJobAsync(company.Id, id);

                ViewBag.JobTitle = vm.JobTitle;

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("ManageJobs");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportApplicants(int id)
        {
            var userIdStr = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");
            int userId = int.Parse(userIdStr);

            try
            {
                var company = await _companyService.GetCompanyByUserIdAsync(userId);
                if (company == null)
                {
                    TempData["Error"] = "Không tìm thấy thông tin công ty.";
                    return RedirectToAction("JobApplicants", new { id = id });
                }

                var fileContent = await _applicationService.ExportApplicantsToExcelAsync(company.Id, id);

                string fileName = $"DanhSachUngVien_Job_{id}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

                return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi xuất file: " + ex.Message;
                return RedirectToAction("JobApplicants", new { id = id });
            }
        }
    }
}
