using System;

namespace SWP391_Project.Models;

public class Notification
{
    public int Id { get; set; }

    /// <summary>
    /// Tiêu đề/thông điệp chính của notification.
    /// </summary>
    public required string Title { get; set; }

    public bool Read { get; set; } = false;

    /// <summary>
    /// Phân loại notification (REPORT/JOB/OTHERS) để xử lý hiển thị.
    /// </summary>
    public NotificationType Type { get; set; } = NotificationType.OTHERS;

    /// <summary>
    /// Id dùng cho deep-link (ví dụ JobId, ReportId...).
    /// </summary>
    public int? NavigationId { get; set; }

    /// <summary>
    /// FK tới Candidate nhận notification.
    /// Nếu sau này cần gửi cho Company/Admin có thể tách thêm bảng/kiểu noti.
    /// </summary>
    public int CandidateId { get; set; }

    /// <summary>
    /// Navigation tới Candidate.
    /// </summary>
    public Candidate Candidate { get; set; } = null!;
}
