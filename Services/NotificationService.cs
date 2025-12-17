using SWP391_Project.Models;
using SWP391_Project.Repositories;
using SWP391_Project.ViewModels.Notification;
using SWP391_Project.Constants;

namespace SWP391_Project.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly IAdminRepository _adminRepository;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        INotificationRepository notificationRepository,
        ICandidateRepository candidateRepository,
        IAdminRepository adminRepository,
        ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _candidateRepository = candidateRepository;
        _adminRepository = adminRepository;
        _logger = logger;
    }

    public async Task CreateReportStatusUpdatedAsync(int reportId, ReportStatus newStatus, string? adminNote)
    {
        try
        {
            var info = await _adminRepository.GetReportNotificationInfoAsync(reportId);
            if (info == null) return;

            var title = NotificationConstants.BuildReportStatusUpdatedMessage(info.JobTitle, newStatus, adminNote);

            var noti = new Notification
            {
                Title = title,
                Type = NotificationType.REPORT,
                CandidateId = info.CandidateId,
                NavigationId = info.JobId,
                Read = false
            };

            await _notificationRepository.AddAsync(noti);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating notification for report {ReportId}", reportId);
        }
    }

    public async Task<(List<NotificationItemVM> Items, int UnreadCount)> GetDropdownForCandidateAsync(int userId, int take)
    {
        var candidate = await _candidateRepository.GetByUserIdAsync(userId);
        if (candidate == null) return (new List<NotificationItemVM>(), 0);

        var unread = await _notificationRepository.CountUnreadForCandidateAsync(candidate.Id);
        var items = await _notificationRepository.GetLatestForCandidateAsync(candidate.Id, take);
        return (items.Select(ToItem).ToList(), unread);
    }

    public async Task<List<NotificationItemVM>> GetAllForCandidateAsync(int userId)
    {
        var candidate = await _candidateRepository.GetByUserIdAsync(userId);
        if (candidate == null) return new List<NotificationItemVM>();

        var items = await _notificationRepository.GetAllForCandidateAsync(candidate.Id);
        return items.Select(ToItem).ToList();
    }

    public async Task<int?> OpenAsync(int userId, int notificationId)
    {
        var candidate = await _candidateRepository.GetByUserIdAsync(userId);
        if (candidate == null) return null;

        var noti = await _notificationRepository.GetByIdForCandidateAsync(notificationId, candidate.Id);
        if (noti == null) return null;

        // Mark read
        await _notificationRepository.MarkReadAsync(notificationId, candidate.Id);

        // Only navigate if NavigationId exists
        return noti.NavigationId;
    }

    private static NotificationItemVM ToItem(Notification n)
    {
        return new NotificationItemVM
        {
            Id = n.Id,
            Title = n.Title,
            Read = n.Read,
            NavigationId = n.NavigationId,
            Type = n.Type
        };
    }
}
