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
        private readonly EzJobDbContext _context;
        private readonly ILocationService _locationService;
        public readonly IEmailService _emailService;
        public readonly IMemoryCache _cache;

        public AccountController(EzJobDbContext context, ILocationService locationService, IEmailService emailService, IMemoryCache cache)
        {
            _context = context;
            _locationService = locationService;
            _emailService = emailService;
            _cache = cache;
            
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
            // Validate Company fields nếu role là COMPANY
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
            //check trung email
            if(_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng");
                var cities = await _locationService.GetCitiesAsync();
                ViewBag.Cities = cities;
                ViewBag.SelectedRole = model.Role;
                return View(model);
            }
            //tao user
            var newUser = new User
            {
                Email = model.Email,
                Password = HashHelper.Hash(model.Password),
                Role = model.Role,
                Active = false
            };
            _context.Users.Add(newUser);
            _context.SaveChanges();
            //tao thong tin cho candidate hoac company
            if(model.Role == Role.CANDIDATE)
            {
                var candidate = new Candidate
                {
                    UserId = newUser.Id,
                    FullName = model.FullName,
                    Jobless = true,
                    RemainingReport = 2
                };
                _context.Candidates.Add(candidate);
            } else if(model.Role == Role.COMPANY)
            {
                // Lấy tên thành phố từ code
                var cities = await _locationService.GetCitiesAsync();
                var selectedCity = cities.FirstOrDefault(c => c.Code == model.City);
                var cityName = selectedCity?.Name ?? model.City!;
                
                // Ward đã là tên rồi, không cần convert
                var wardName = model.Ward!;
                
                // Tìm hoặc tạo Location từ City và Ward
                var location = _context.Locations
                    .FirstOrDefault(l => l.City == cityName && l.Ward == wardName);
                
                if(location == null)
                {
                    location = new Location
                    {
                        City = cityName,
                        Ward = wardName
                    };
                    _context.Locations.Add(location);
                    _context.SaveChanges();
                }

                var company = new Company
                {
                    UserId = newUser.Id,
                    Name = model.FullName,
                    Description = model.Description!,
                    Address = model.Address!,
                    PhoneNumber = model.PhoneNumber!,
                    LocationId = location.Id
                };
                _context.Add(company);
            }
            _context.SaveChanges();

            string token = Guid.NewGuid().ToString();

            //set thoi gian cho xac thuc la 1 ngfay
            _cache.Set("Verify_" + token, newUser.Email, TimeSpan.FromHours(24));

            string verifyLink = Url.Action("VerifyAccount", "Account",
                new { token = token }, Request.Scheme);

            string subject = "Xác thực tài khoản - EZJob";
            string body = $@"
                <h3>Chào mừng bạn đến với EZJob!</h3>
                <p>Bạn đã đăng ký tài khoản thành công.</p>
                <p>Vui lòng <a href='{verifyLink}'>BẤM VÀO ĐÂY</a> để kích hoạt tài khoản.</p>";

            try
            {
                _emailService.SendMail(newUser.Email, subject, body);
            }
            catch
            {
            }
            return View("RegisterConfirmation");
        }

        [HttpGet]
        public IActionResult VerifyAccount(string token)
        {
            if(!_cache.TryGetValue("Verify_" + token, out string email))
            {
                ViewBag.Error = "Link xác thực không hợp lệ hoặc đã hết hạn";
                return View("Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if(user != null)
            {
                if(user.Active == true)
                {
                    TempData["Success"] = "Tài khoản này đã được kích hoạt trước đó rồi.";
                }
                else
                {
                    user.Active = true;
                    _context.SaveChanges();

                    _cache.Remove("Verify_" + token);
                    TempData["Success"] = "Kích hoạt tài khoản thành công! Bạn có thể đăng nhập ngay.";
                }
            }

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
        public IActionResult Login(LoginVM model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if(user != null && HashHelper.Verify(model.Password, user.Password))
            {
                if(user.Active == false)
                {
                    ViewBag.Error = "Tài khoản hiện đang bị khóa hoặc chưa được kích hoạt.";
                    return View();
                }
                
                HttpContext.Session.SetString("UserID", user.Id.ToString());
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("Role", user.Role.ToString());

                string displayName = user.Email; 
                string? imageUrl = null;

                if(user.Role == Role.CANDIDATE)
                {
                    var can = _context.Candidates.FirstOrDefault(c => c.UserId == user.Id);
                    if (can != null)
                    {
                        displayName = can.FullName;
                        imageUrl = can.ImageUrl;
                    }
                } else if( user.Role == Role.COMPANY)
                {
                    var com = _context.Companies.FirstOrDefault(c => c.UserId == user.Id);
                    if (com != null)
                    {
                        displayName = com.Name;
                        imageUrl = com.ImageUrl;
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
                } else if(user.Role == Role.COMPANY)
                {
                    return RedirectToAction("Index", "Company");
                }
                else
                {
                    return RedirectToAction("Index", "Candidate");
                }
            }
            ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
            return View(model);
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
        public IActionResult ForgotPassword(ForgotPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

            if(user != null){
                string token = Guid.NewGuid().ToString();

                _cache.Set(token, user.Email, TimeSpan.FromMinutes(15));

                string resetLink = Url.Action("ResetPassword", "Account", new { token = token }, Request.Scheme);

                //gui mail cho user lay link
                string subject = "Yeu cau dat lai mat khau - EZJob";
                string body = $@"
                    <h3>Xin chào,</h3>
                    <p>Bạn đã yêu cầu đặt lại mật khẩu.</p>
                    <p>Vui lòng <a href='{resetLink}'>BẤM VÀO ĐÂY</a> để tạo mật khẩu mới.</p>
                    <p><i>Link này chỉ có hiệu lực trong 15 phút.</i></p>";

                try
                {
                    _emailService.SendMail(user.Email, subject, body);
                }
                catch
                {
                    ModelState.AddModelError("", "Lỗi gửi email, vui lòng thử lại");
                    return View(model);
                }
            }
            return View("ForgotPasswordConfirmation");
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if(string.IsNullOrEmpty(token) || !_cache.TryGetValue(token, out string email))
            {
                ViewBag.Error = "Đường dẫn đătj lại mật khẩu không hợp lệ hoặc đã hết hạn";
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
        public IActionResult ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!_cache.TryGetValue(model.Token, out string emailFromCache))
            {
                ViewBag.Error = "Phiên làm việc đã hết hạn. Vui lòng thực hiện lại.";
                return View(model);
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if(user != null)
            {
                user.Password = HashHelper.Hash(model.NewPassword);
                _context.SaveChanges();

                _cache.Remove(model.Token);

                TempData["Success"] = "Đổi mật khẩu thành công! Vui lòng đăng nhập lại.";
                return RedirectToAction("Login");
            }

            ViewBag.Error = "Có lỗi xảy ra, không tìm thấy tài khoản";
            return View(model);
        }
    }
}
