using System.Collections.Generic;

namespace SWP391_Project.ViewModels.Notification;

public class NotificationDropdownVM
{
    public int UnreadCount { get; set; }
    public List<NotificationItemVM> Items { get; set; } = new();
}
