using Microsoft.AspNetCore.Mvc;
using SWP391_Project.Services;
using SWP391_Project.ViewModels.Notification;

namespace SWP391_Project.ViewComponents;

public class NotificationDropdownViewComponent : ViewComponent
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly INotificationService _notificationService;

    public NotificationDropdownViewComponent(IHttpContextAccessor httpContextAccessor, INotificationService notificationService)
    {
        _httpContextAccessor = httpContextAccessor;
        _notificationService = notificationService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int take = 5)
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        var userIdStr = session?.GetString("UserID");
        var role = session?.GetString("Role") ?? string.Empty;

        if (!string.Equals(role, "CANDIDATE", StringComparison.OrdinalIgnoreCase))
        {
            // Not a candidate -> do not render this component.
            return Content(string.Empty);
        }

        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
        {
            return View(new NotificationDropdownVM());
        }

        var (items, unread) = await _notificationService.GetDropdownForCandidateAsync(userId, take);
        return View(new NotificationDropdownVM { Items = items, UnreadCount = unread });
    }
}
