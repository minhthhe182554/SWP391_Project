using Microsoft.AspNetCore.Mvc;
using SWP391_Project.Services;
using SWP391_Project.ViewModels.Jobs;
using System.Threading.Tasks;

namespace SWP391_Project.Controllers
{
    public class JobController : Controller
    {
        private readonly IJobService _jobService;

        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }

        public async Task<IActionResult> Detail(int id)
        {
            int? userId = null;
            var userIdStr = HttpContext.Session.GetString("UserID");

            if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int parsedId))
            {
                userId = parsedId;
            }

            var vm = await _jobService.GetJobDetailAsync(id, userId);

            if (vm == null) return NotFound();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReport(int jobId, string reason)
        {
            var role = HttpContext.Session.GetString("Role");
            var userIdStr = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(role) || role != "CANDIDATE" || string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            {
                return Json(new { success = false, requiresLogin = true, message = "Vui lòng đăng nhập bằng tài khoản ứng viên" });
            }

            var result = await _jobService.CreateJobReportAsync(jobId, userId, reason);
            return Json(new { success = result.Success, message = result.Message });
        }
    }
}

