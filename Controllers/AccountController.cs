using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SWP391_Project.Dtos;
using SWP391_Project.Helpers;
using SWP391_Project.Models;
using SWP391_Project.Services;
using SWP391_Project.ViewModels;

namespace SWP391_Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ILocationService _locationService;
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IAccountService accountService,
            ILocationService locationService,
            IEmailService emailService,
            IMemoryCache cache,
            ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _locationService = locationService;
            _emailService = emailService;
            _cache = cache;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Register(string? role)
        {
            var cities = await _locationService.GetCitiesAsync();
            ViewBag.Cities = cities ?? new List<CityDto>();

            Role? selectedRole = null;
            if (!string.IsNullOrEmpty(role) && Enum.TryParse<Role>(role, true, out var parsed))
            {
                selectedRole = parsed;
            }

            ViewBag.SelectedRole = selectedRole;

            var vm = new RegisterVM();
            if (selectedRole.HasValue)
            {
                vm.Role = selectedRole.Value;
            }
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> GetWards(string cityCode)
        {
            if (string.IsNullOrEmpty(cityCode))
            {
                return Json(new List<object>());
            }
            
            var wards = await _locationService.GetWardsByCityCodeAsync(cityCode);
            return Json(wards.Select(w => new { code = w.Code, name = w.Name }));
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if(model.Role == Role.COMPANY)
            {
                if(string.IsNullOrWhiteSpace(model.PhoneNumber))
                {
                    ModelState.AddModelError("PhoneNumber", "Vui lòng nhập số điện thoại");
                }
                if(string.IsNullOrWhiteSpace(model.Description))
                {
                    ModelState.AddModelError("Description", "Vui lòng nhập mô tả công ty");
                }
                if(string.IsNullOrWhiteSpace(model.Address))
                {
                    ModelState.AddModelError("Address", "Vui lòng nhập địa chỉ");
                }
                if(string.IsNullOrWhiteSpace(model.City))
                {
                    ModelState.AddModelError("City", "Vui lòng chọn thành phố");
                }
                if(string.IsNullOrWhiteSpace(model.Ward))
                {
                    ModelState.AddModelError("Ward", "Vui lòng chọn phường/xã");
                }
            }

            if(!ModelState.IsValid)
            {
                var cities = await _locationService.GetCitiesAsync();
                ViewBag.Cities = cities;
                ViewBag.SelectedRole = model.Role;
                return View(model);
            }

            // Register based on role
            var (success, error) = model.Role == Role.CANDIDATE
                ? await _accountService.RegisterCandidateAsync(model)
                : await _accountService.RegisterCompanyAsync(model, SendVerificationEmailAsync);

            if (!success)
            {
                ModelState.AddModelError("Email", error ?? "Đăng ký không thành công");
                var cities = await _locationService.GetCitiesAsync();
                ViewBag.Cities = cities;
                ViewBag.SelectedRole = model.Role;
                return View(model);
            }

            // Send verification email
            var user = await _accountService.GetUserByEmailAsync(model.Email);
            if (user != null)
            {
                string token = Guid.NewGuid().ToString();
                _cache.Set("Verify_" + token, user.Email, TimeSpan.FromHours(24));

                try
                {
                    await SendVerificationEmailAsync(user.Email, token);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending verification email");
                }
            }
            return View("RegisterConfirmation");
        }

        private async Task SendVerificationEmailAsync(string email, string token)
        {
            string verifyLink = Url.Action("VerifyAccount", "Account",
                new { token = token }, Request.Scheme) ?? "";

            string subject = "Xác thực tài khoản - EZJob";
            string body = $@"
                <h3>Chào mừng bạn đến với EZJob!</h3>
                <p>Bạn đã đăng ký tài khoản thành công.</p>
                <p>Vui lòng <a href='{verifyLink}'>BẤM VÀO ĐÂY</a> để kích hoạt tài khoản.</p>";

            await Task.Run(() => _emailService.SendMail(email, subject, body));
        }

        [HttpGet]
        public IActionResult VerifyAccount(string token)
        {
            if(!_cache.TryGetValue("Verify_" + token, out string? email) || string.IsNullOrEmpty(email))
            {
                ViewBag.Error = "Link xác thực không hợp lệ hoặc đã hết hạn";
                return View("Login");
            }

            return RedirectToAction("VerifyAccountPost", new { token, email });
        }

        [HttpGet]
        public async Task<IActionResult> VerifyAccountPost(string token, string email)
        {
            var (success, error) = await _accountService.VerifyAccountAsync(token, email);

            if (!success)
            {
                TempData["Success"] = error ?? "Không thể xác thực tài khoản";
                return RedirectToAction("Login");
            }

            _cache.Remove("Verify_" + token);
            TempData["Success"] = "Kích hoạt tài khoản thành công! Bạn có thể đăng nhập ngay.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            if(HttpContext.Session.GetString("Email") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, user, error) = await _accountService.LoginAsync(model);

            if (!success || user == null)
            {
                ViewBag.Error = error ?? "Có lỗi xảy ra";
                return View(model);
            }

            // Set session
            HttpContext.Session.SetString("UserID", user.Id.ToString());
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("Role", user.Role.ToString());

            // Set user display name and image
            string displayName = user.Email;
            string? imageUrl = null;

            if(user.Role == Role.CANDIDATE)
            {
                var candidate = await _accountService.GetCandidateByUserIdAsync(user.Id);
                if (candidate != null)
                {
                    displayName = candidate.FullName;
                    imageUrl = candidate.ImageUrl;
                }
            } 
            else if(user.Role == Role.COMPANY)
            {
                var company = await _accountService.GetCompanyByUserIdAsync(user.Id);
                if (company != null)
                {
                    displayName = company.Name;
                    imageUrl = company.ImageUrl;
                }
            }

            HttpContext.Session.SetString("Name", displayName);
            if (!string.IsNullOrEmpty(imageUrl))
            {
                HttpContext.Session.SetString("ImageUrl", imageUrl);
            }

            if(user.Role == Role.ADMIN)
            {
                return RedirectToAction("Index", "Admin");
            } 
            else if(user.Role == Role.COMPANY)
            {
                return RedirectToAction("Index", "Company");
            }
            else
            {
                // Candidate: show Home with jobs + companies sections
                return RedirectToAction("Index", "Home");
            }
        }
        
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _accountService.GetUserByEmailAsync(model.Email);

            if(user != null)
            {
                string token = Guid.NewGuid().ToString();
                _cache.Set(token, user.Email, TimeSpan.FromMinutes(15));

                try
                {
                    string resetLink = Url.Action("ResetPassword", "Account", new { token = token }, Request.Scheme) ?? "";
                    string subject = "Yeu cau dat lai mat khau - EZJob";
                    string body = $@"
                        <h3>Xin chào,</h3>
                        <p>Bạn đã yêu cầu đặt lại mật khẩu.</p>
                        <p>Vui lòng <a href='{resetLink}'>BẤM VÀO ĐÂY</a> để tạo mật khẩu mới.</p>
                        <p><i>Link này chỉ có hiệu lực trong 15 phút.</i></p>";

                    await Task.Run(() => _emailService.SendMail(user.Email, subject, body));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending password reset email");
                    ModelState.AddModelError("", "Lỗi gửi email, vui lòng thử lại");
                    return View(model);
                }
            }
            
            return View("ForgotPasswordConfirmation");
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if(string.IsNullOrEmpty(token) || !_cache.TryGetValue(token, out string? email) || string.IsNullOrEmpty(email))
            {
                ViewBag.Error = "Đường dẫn đặt lại mật khẩu không hợp lệ hoặc đã hết hạn";
                return View(new ResetPasswordVM());
            }
            
            var model = new ResetPasswordVM
            {
                Token = token,
                Email = email
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!_cache.TryGetValue(model.Token, out string? emailFromCache) || string.IsNullOrEmpty(emailFromCache))
            {
                ViewBag.Error = "Phiên làm việc đã hết hạn. Vui lòng thực hiện lại.";
                return View(model);
            }

            var (success, error) = await _accountService.ResetPasswordAsync(model.Email, model.NewPassword);
            
            if (!success)
            {
                ViewBag.Error = error ?? "Có lỗi xảy ra";
                return View(model);
            }

            _cache.Remove(model.Token);
            TempData["Success"] = "Đổi mật khẩu thành công! Vui lòng đăng nhập lại.";
            return RedirectToAction("Login");
        }
    }
}
