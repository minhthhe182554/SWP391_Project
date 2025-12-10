using Microsoft.AspNetCore.Mvc;
using SWP391_Project.Helpers;
using SWP391_Project.Models;
using SWP391_Project.Services;

namespace SWP391_Project.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
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
    }
}
