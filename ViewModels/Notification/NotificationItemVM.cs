using SWP391_Project.Models;

namespace SWP391_Project.ViewModels.Notification;

public class NotificationItemVM
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool Read { get; set; }
    public NotificationType Type { get; set; }
    public int? NavigationId { get; set; }
}
