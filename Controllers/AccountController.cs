using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWP391_Project.Helpers;
using SWP391_Project.Models;
using SWP391_Project.ViewModels;

namespace SWP391_Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly EzJobDbContext _context;

        public AccountController(EzJobDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterVM model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            //check trung email
            if(_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email nay da duoc su dung");
                return View(model);
            }
            //tao user
            var newUser = new User
            {
                Email = model.Email,
                Password = HashHelper.Hash(model.Password),
                Role = model.Role,
                Active = true
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
                var company = new Company
                {
                    UserId = newUser.Id,
                    Name = model.FullName,
                    Description = "Chua cap nhat mo ta",
                    Address = "Chua cap nhat dia chi",
                    PhoneNumber = "0122222222",
                    LocationId = 1
                };
                _context.Add(company);
            }
            _context.SaveChanges();
            // 4. Chuyen ve login
            TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
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
                    ViewBag.Error = "Tai khoan da bi khoa";
                    return View();
                }
                
                HttpContext.Session.SetString("UserID", user.Id.ToString());
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("Role", user.Role.ToString());

                string displayName = user.Email; 

                if(user.Role == Role.CANDIDATE)
                {
                    var can = _context.Candidates.FirstOrDefault(c => c.UserId == user.Id);
                    if (can != null)
                    {
                        displayName = can.FullName;
                    }
                } else if( user.Role == Role.COMPANY)
                {
                    var com = _context.Companies.FirstOrDefault(c => c.UserId == user.Id);
                    if (com != null)
                    {
                        displayName = com.Name;
                    }
                }
                HttpContext.Session.SetString("Name", displayName);

                if(user.Role == Role.ADMIN)
                {
                    return RedirectToAction("Dashboard", "Admin");
                } else if(user.Role == Role.COMPANY)
                {
                    return RedirectToAction("Dashboard", "Company");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            ViewBag.Error = "Sai tai khoan hoac mat khau";
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
