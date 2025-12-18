using SWP391_Project.Models.Enums;

namespace SWP391_Project.Constants;

public static class NotificationConstants
{
    public const int DropdownTake = 5;

    public const string ReportStatusNotePlaceholder = "Nhập nội dung thông báo (tuỳ chọn). Ví dụ: Việc này đang được hệ thống điều tra...";

    public static string BuildReportStatusUpdatedMessage(string jobTitle, ReportStatus status, string? adminNote)
    {
        var title = $"Cập nhật trạng thái báo cáo: \"{jobTitle}\"";
        var statusLine = $"Trạng thái: {EnumText.ToVietnamese(status)}";

        var note = (adminNote ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(note))
        {
            return title + "\n" + statusLine;
        }

        return title + "\n" + statusLine + "\n\n" + "Ghi chú từ quản trị:" + "\n" + note;
    }

    public static string BuildReportDeletedMessage(string jobTitle)
    {
        return $"Bạn đã xóa báo cáo về công việc: \"{jobTitle}\"";
    }
}
