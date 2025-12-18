using Microsoft.AspNetCore.Mvc;
using SWP391_Project.Helpers;
using SWP391_Project.Models.Enums;
using SWP391_Project.Services;

namespace SWP391_Project.Controllers;

[RoleAuthorize(Role.CANDIDATE)]
public class NotificationController : Controller
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userIdStr = HttpContext.Session.GetString("UserID");
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var items = await _notificationService.GetAllForCandidateAsync(userId);
        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Open(int id)
    {
        var userIdStr = HttpContext.Session.GetString("UserID");
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var jobId = await _notificationService.OpenAsync(userId, id);
        if (jobId.HasValue)
        {
            return RedirectToAction("Detail", "Job", new { id = jobId.Value });
        }

        // No navigation target
        return NoContent();
    }
}
