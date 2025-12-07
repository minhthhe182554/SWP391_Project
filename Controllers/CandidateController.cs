using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391_Project.Helpers;
using SWP391_Project.Models;
using SWP391_Project.ViewModels;

namespace SWP391_Project.Controllers
{
    public class CandidateController : Controller
    {
        private readonly EzJobDbContext _context;

        public CandidateController(EzJobDbContext context)
        {
            _context = context;
        }

        [RoleAuthorize(Role.CANDIDATE)]
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var candidate = _context.Candidates
                .Include(c => c.User)
                .FirstOrDefault(c => c.UserId == int.Parse(userId));
            if (candidate == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Update session với thông tin mới nhất từ database
            HttpContext.Session.SetString("Name", candidate.FullName);
            if (!string.IsNullOrEmpty(candidate.ImageUrl))
            {
                HttpContext.Session.SetString("ImageUrl", candidate.ImageUrl);
            }

            // Get recommended jobs (to be implemented with matching algorithm)
            var recommendedJobs = _context.Jobs
                .Include(j => j.Company)
                .Include(j => j.Location)
                .Include(j => j.RequiredSkills)
                .Where(j => j.EndDate >= DateTime.Now && !j.IsDelete)
                .OrderByDescending(j => j.StartDate)
                .Take(10)
                .ToList();

            // Get all active jobs
            var allJobs = _context.Jobs
                .Include(j => j.Company)
                .Include(j => j.Location)
                .Include(j => j.RequiredSkills)
                .Where(j => j.EndDate >= DateTime.Now && !j.IsDelete)
                .OrderByDescending(j => j.StartDate)
                .ToList();

            var viewModel = new CandidateHomeVM
            {
                RecommendedJobs = recommendedJobs,
                AllJobs = allJobs
            };

            return View(viewModel);
        }
    }
}
