using Microsoft.AspNetCore.Mvc;
using SWP391_Project.Helpers;
using SWP391_Project.Models;
using SWP391_Project.Services;

namespace SWP391_Project.Controllers
{
    public class CandidateController : Controller
    {
        private readonly ICandidateService _candidateService;

        public CandidateController(ICandidateService candidateService)
        {
            _candidateService = candidateService;
        }

        [RoleAuthorize(Role.CANDIDATE)]
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

            // Update session với thông tin mới nhất từ database
            HttpContext.Session.SetString("Name", candidate.FullName);
            if (!string.IsNullOrEmpty(candidate.ImageUrl))
            {
                HttpContext.Session.SetString("ImageUrl", candidate.ImageUrl);
            }

            // Get home view with jobs
            var viewModel = await _candidateService.GetCandidateHomeViewAsync(userIdInt);

            return View(viewModel);
        }
    }
}
