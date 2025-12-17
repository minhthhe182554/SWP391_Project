using SWP391_Project.Models;

namespace SWP391_Project.Constants;

public static class EnumText
{
    public static string ToVietnamese(Role role)
    {
        return role switch
        {
            Role.ADMIN => "Quản trị",
            Role.CANDIDATE => "Ứng viên",
            Role.COMPANY => "Công ty",
            _ => role.ToString()
        };
    }

    public static string ToVietnamese(ReportStatus status)
    {
        return status switch
        {
            ReportStatus.PENDING => "Chờ duyệt",
            ReportStatus.REVIEWING => "Đang xử lý",
            ReportStatus.RESOLVED => "Đã xử lý",
            _ => status.ToString()
        };
    }

    public static string ToVietnamese(JobType type)
    {
        return type switch
        {
            JobType.FULLTIME => "Toàn thời gian",
            JobType.PARTTIME => "Bán thời gian",
            JobType.HYBRID => "Hybrid",
            _ => type.ToString()
        };
    }
}
