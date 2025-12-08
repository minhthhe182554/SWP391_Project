using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391_Project.Helpers;
using SWP391_Project.Models;
using SWP391_Project.ViewModels;

namespace SWP391_Project.Controllers
{
    public class AdminController : Controller
    {
        private readonly EzJobDbContext _context;

        public AdminController(EzJobDbContext context)
        {
            _context = context;
        }

        [RoleAuthorize(Role.ADMIN)]
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == int.Parse(userId));
            if (user != null)
            {
                HttpContext.Session.SetString("Name", user.Email);
            }

            // Get system statistics
            var totalUsers = _context.Users.Count();
            var totalCandidates = _context.Candidates.Count();
            var totalCompanies = _context.Companies.Count();
            var totalJobs = _context.Jobs.IgnoreQueryFilters().Count();
            var totalApplications = _context.Applications.Count();
            var totalReports = _context.Reports.Count();
            var pendingReports = _context.Reports.Count(r => r.Status == ReportStatus.PENDING);

            var viewModel = new AdminDashboardVM
            {
                TotalUsers = totalUsers,
                TotalCandidates = totalCandidates,
                TotalCompanies = totalCompanies,
                TotalJobs = totalJobs,
                TotalApplications = totalApplications,
                TotalReports = totalReports,
                PendingReports = pendingReports
            };

            return View(viewModel);
        }
    }
}
