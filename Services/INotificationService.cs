using SWP391_Project.Models;
using SWP391_Project.ViewModels.Notification;

namespace SWP391_Project.Services;

public interface INotificationService
{
    Task CreateReportStatusUpdatedAsync(int reportId, ReportStatus newStatus, string? adminNote);
    Task<(List<NotificationItemVM> Items, int UnreadCount)> GetDropdownForCandidateAsync(int userId, int take);
    Task<List<NotificationItemVM>> GetAllForCandidateAsync(int userId);
    Task<int?> OpenAsync(int userId, int notificationId);
}
