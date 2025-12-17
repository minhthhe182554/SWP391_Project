using SWP391_Project.Models;
using SWP391_Project.ViewModels;
using SWP391_Project.ViewModels.Admin;

namespace SWP391_Project.Services
{
    public interface IAdminService
    {
        Task<User?> GetAdminUserByIdAsync(int userId);
        Task<AdminDashboardVM> GetDashboardMetricsAsync(int days);
        Task<ManageUsersVM> GetManageUsersAsync(
            int candidateActivePage,
            int candidateBannedPage,
            int companyActivePage,
            int companyBannedPage,
            int pageSize,
            int? focusUserId = null);
        Task<ManageReportsVM> GetManageReportsAsync(int page, int pageSize, int? focusReportId = null);
        Task<ManageJobsAdminVM> GetManageJobsAsync(int page, int pageSize, string statusFilter, int? focusJobId);
        Task<AdminJobDetailVM?> GetJobDetailForAdminAsync(int jobId);
        Task UpdateReportStatusAsync(int reportId, ReportStatus status, string? adminNote = null);
        Task<UserDetailVM?> GetUserDetailAsync(int userId);
        Task ToggleUserActiveAsync(int userId, bool active);
    }
}
