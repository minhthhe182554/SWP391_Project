using Microsoft.AspNetCore.Mvc;
using SWP391_Project.Helpers;
using SWP391_Project.Models;
using SWP391_Project.Services;
using SWP391_Project.ViewModels.Candidate;
using System.ComponentModel.DataAnnotations;

namespace SWP391_Project.Controllers
{
    [RoleAuthorize(Role.CANDIDATE)]
    public class CandidateController : Controller
    {
        private readonly ICandidateService _candidateService;
        private readonly ICloudinaryHelper _cloudinaryHelper;

        public CandidateController(ICandidateService candidateService, ICloudinaryHelper cloudinaryHelper)
        {
            _candidateService = candidateService;
            _cloudinaryHelper = cloudinaryHelper;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var userIdInt = int.Parse(userId);
            var candidate = await _candidateService.GetCandidateByUserIdAsync(userIdInt);
            if (candidate == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Update session 
            HttpContext.Session.SetString("Name", candidate.FullName);
            if (!string.IsNullOrEmpty(candidate.ImageUrl))
            {
                HttpContext.Session.SetString("ImageUrl", candidate.ImageUrl);
            }

            // Get home view with jobs
            var viewModel = await _candidateService.GetCandidateHomeViewAsync(userIdInt);

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Resume()
        {
            System.Console.WriteLine("Quan ly resume");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var userIdInt = int.Parse(userId);
            var viewModel = await _candidateService.GetProfileAsync(userIdInt);
            if (viewModel == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.AllSkills = await _candidateService.GetAllSkillsAsync();
            ViewBag.ImageUrl = viewModel.ImageUrl != null 
                ? _cloudinaryHelper.BuildImageUrl(viewModel.ImageUrl) 
                : _cloudinaryHelper.BuildImageUrl("default_yvl9oh");

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string fullName, string? phoneNumber)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            if (string.IsNullOrWhiteSpace(fullName))
            {
                return Json(new { success = false, message = "Họ và tên không được để trống" });
            }

            // Validate phone number if provided
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                var phoneRegex = new System.Text.RegularExpressions.Regex(@"^[0-9]{10,11}$");
                if (!phoneRegex.IsMatch(phoneNumber))
                {
                    return Json(new { success = false, message = "Số điện thoại không hợp lệ" });
                }
            }

            var userIdInt = int.Parse(userId);
            var success = await _candidateService.UpdateProfileAsync(userIdInt, fullName, phoneNumber);
            
            if (success)
            {
                HttpContext.Session.SetString("Name", fullName);
            }

            return Json(new { success, message = success ? "Cập nhật thành công" : "Cập nhật thất bại" });
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                return Json(new { success = false, message = "Mật khẩu mới phải có ít nhất 6 ký tự" });
            }

            if (newPassword != confirmPassword)
            {
                return Json(new { success = false, message = "Mật khẩu xác nhận không khớp" });
            }

            var userIdInt = int.Parse(userId);
            var hashedPassword = HashHelper.Hash(newPassword);
            var success = await _candidateService.UpdatePasswordAsync(userIdInt, hashedPassword);

            return Json(new { success, message = success ? "Đổi mật khẩu thành công" : "Đổi mật khẩu thất bại" });
        }

        [HttpPost]
        public async Task<IActionResult> AddEducationRecord(string title, string startDate, string? endDate)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                return Json(new { success = false, message = "Tiêu đề không được để trống" });
            }

            var userIdInt = int.Parse(userId);
            var candidate = await _candidateService.GetCandidateByUserIdAsync(userIdInt);
            if (candidate == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin ứng viên" });
            }

            if (!DateTime.TryParse(startDate, out var startDateParsed))
            {
                return Json(new { success = false, message = "Ngày bắt đầu không hợp lệ" });
            }

            DateTime? endDateParsed = null;
            if (!string.IsNullOrWhiteSpace(endDate) && DateTime.TryParse(endDate, out var parsed))
            {
                endDateParsed = parsed;
            }

            var record = new EducationRecord
            {
                Title = title,
                StartDate = startDateParsed,
                EndDate = endDateParsed
            };

            var success = await _candidateService.AddEducationRecordAsync(candidate.Id, record);
            return Json(new { success, message = success ? "Thêm thành công" : "Thêm thất bại" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEducationRecord(int recordId)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            var userIdInt = int.Parse(userId);
            var candidate = await _candidateService.GetCandidateByUserIdAsync(userIdInt);
            if (candidate == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin ứng viên" });
            }

            var success = await _candidateService.DeleteEducationRecordAsync(candidate.Id, recordId);
            return Json(new { success, message = success ? "Xóa thành công" : "Xóa thất bại" });
        }

        [HttpPost]
        public async Task<IActionResult> AddWorkExperience(string name, string? description, string startDate, string endDate)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return Json(new { success = false, message = "Tên công việc không được để trống" });
            }

            if (!DateTime.TryParse(startDate, out var startDateParsed))
            {
                return Json(new { success = false, message = "Ngày bắt đầu không hợp lệ" });
            }

            if (!DateTime.TryParse(endDate, out var endDateParsed))
            {
                return Json(new { success = false, message = "Ngày kết thúc không hợp lệ" });
            }

            if (endDateParsed < startDateParsed)
            {
                return Json(new { success = false, message = "Ngày kết thúc phải sau ngày bắt đầu" });
            }

            var userIdInt = int.Parse(userId);
            var candidate = await _candidateService.GetCandidateByUserIdAsync(userIdInt);
            if (candidate == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin ứng viên" });
            }

            var experience = new WorkExperience
            {
                Name = name,
                Description = description,
                StartDate = startDateParsed,
                EndDate = endDateParsed
            };

            var success = await _candidateService.AddWorkExperienceAsync(candidate.Id, experience);
            return Json(new { success, message = success ? "Thêm thành công" : "Thêm thất bại" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteWorkExperience(int experienceId)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            var userIdInt = int.Parse(userId);
            var candidate = await _candidateService.GetCandidateByUserIdAsync(userIdInt);
            if (candidate == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin ứng viên" });
            }

            var success = await _candidateService.DeleteWorkExperienceAsync(candidate.Id, experienceId);
            return Json(new { success, message = success ? "Xóa thành công" : "Xóa thất bại" });
        }

        [HttpPost]
        public async Task<IActionResult> AddSkill(int skillId)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            var userIdInt = int.Parse(userId);
            var candidate = await _candidateService.GetCandidateByUserIdAsync(userIdInt);
            if (candidate == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin ứng viên" });
            }

            var success = await _candidateService.AddSkillAsync(candidate.Id, skillId);
            return Json(new { success, message = success ? "Thêm thành công" : "Thêm thất bại" });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveSkill(int skillId)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            var userIdInt = int.Parse(userId);
            var candidate = await _candidateService.GetCandidateByUserIdAsync(userIdInt);
            if (candidate == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin ứng viên" });
            }

            var success = await _candidateService.RemoveSkillAsync(candidate.Id, skillId);
            return Json(new { success, message = success ? "Xóa thành công" : "Xóa thất bại" });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateJoblessStatus(bool jobless)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            var userIdInt = int.Parse(userId);
            var success = await _candidateService.UpdateJoblessStatusAsync(userIdInt, jobless);

            return Json(new { success, message = success ? "Cập nhật thành công" : "Cập nhật thất bại" });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAndAddSkill(string skillName)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            if (string.IsNullOrWhiteSpace(skillName))
            {
                return Json(new { success = false, message = "Tên kỹ năng không hợp lệ" });
            }

            var userIdInt = int.Parse(userId);
            var candidate = await _candidateService.GetCandidateByUserIdAsync(userIdInt);
            if (candidate == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin ứng viên" });
            }

            var success = await _candidateService.CreateAndAddSkillAsync(candidate.Id, skillName);

            return Json(new { success, message = success ? "Thêm kỹ năng mới thành công" : "Thêm kỹ năng thất bại" });
        }

        [HttpPost]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = "Chưa chọn file" });
            }

            try
            {
                var userIdInt = int.Parse(userId);
                var candidate = await _candidateService.GetCandidateByUserIdAsync(userIdInt);
                if (candidate == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin ứng viên" });
                }

                // Use custom public ID based on user ID to enable overwriting
                // Format: user_{userId} - this will always be the same for each user
                var customPublicId = $"user_{userIdInt}";
                
                // Upload to Cloudinary (will overwrite if already exists)
                var publicId = await _cloudinaryHelper.UploadImageAsync(file, "profile-images", customPublicId);

                // Update database only if it's different (first time upload or changed)
                if (candidate.ImageUrl != publicId)
                {
                    var success = await _candidateService.UpdateProfileImageAsync(candidate.Id, publicId);
                    
                    if (!success)
                    {
                        return Json(new { success = false, message = "Cập nhật ảnh đại diện thất bại" });
                    }
                }

                // Update session with new image public ID
                HttpContext.Session.SetString("ImageUrl", publicId);
                
                var imageUrl = _cloudinaryHelper.BuildImageUrl(publicId);
                return Json(new { success = true, message = "Cập nhật ảnh đại diện thành công", imageUrl });
            }
            catch (ArgumentException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi upload ảnh" });
            }
        }
    }
}
