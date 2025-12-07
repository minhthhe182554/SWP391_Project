using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391_Project.Helpers;
using SWP391_Project.Models;
using SWP391_Project.ViewModels;

namespace SWP391_Project.Controllers
{
    public class CompanyController : Controller
    {
        private readonly EzJobDbContext _context;

        public CompanyController(EzJobDbContext context)
        {
            _context = context;
        }

        [RoleAuthorize(Role.COMPANY)]
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var company = _context.Companies
                .Include(c => c.User)
                .FirstOrDefault(c => c.UserId == int.Parse(userId));
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

            // Get company statistics
            var totalJobs = _context.Jobs.Count(j => j.CompanyId == company.Id && !j.IsDelete);
            var activeJobs = _context.Jobs.Count(j => j.CompanyId == company.Id && j.EndDate >= DateTime.Now && !j.IsDelete);
            var totalApplications = _context.Applications
                .Count(a => _context.Jobs.Any(j => j.CompanyId == company.Id && j.Id == a.JobId));

            var viewModel = new CompanyDashboardVM
            {
                TotalJobs = totalJobs,
                ActiveJobs = activeJobs,
                TotalApplications = totalApplications
            };

            return View(viewModel);
        }
    }
}
