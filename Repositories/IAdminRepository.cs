using SWP391_Project.Models;
using SWP391_Project.Models.Enums;

namespace SWP391_Project.Repositories
{
    public interface IAdminRepository
    {
        Task<User?> GetUserByIdAsync(int userId);
        Task<Dictionary<DateTime, int>> GetNewUsersByDateAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<DateTime, int>> GetApplicationsByDateAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<DateTime, int>> GetActiveJobsByDateAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<DateTime, int>> GetActiveCompaniesByDateAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<string, int>> GetTopJobCategoriesAsync(DateTime startDate, DateTime endDate, int take);

        Task<(List<User> Users, int Total)> GetUsersByRolePagedAsync(Role role, int page, int pageSize);
        Task<(List<User> Users, int Total)> GetUsersByRoleAndActivePagedAsync(Role role, bool active, int page, int pageSize);
        Task<int> CountUsersByRoleAsync(Role role);
        Task<int> CountActiveUsersByRoleAsync(Role role);
        Task<int> CountInactiveUsersByRoleAsync(Role role);

        Task<(List<Report> Reports, int Total)> GetJobReportsPagedAsync(int page, int pageSize);
        Task<int?> GetReportPageByIdAsync(int reportId, int pageSize);
        Task UpdateReportStatusAsync(int reportId, ReportStatus status);
        Task<int> CountJobsAsync();
        Task<int> CountActiveJobsAsync();

        Task<(List<Job> Jobs, int Total)> GetJobsPagedAsync(int page, int pageSize, string statusFilter);
        Task<List<Job>> GetReportedJobsAsync(string statusFilter);
        Task<Job?> GetJobDetailForAdminAsync(int jobId);

        Task<User?> GetUserWithProfileAsync(int userId);
        Task UpdateUserActiveAsync(int userId, bool active);
        Task<(Role Role, bool Active)?> GetUserRoleAndActiveAsync(int userId);
        Task<int?> GetUserPageByRoleActiveAsync(Role role, bool active, int userId, int pageSize);

        Task<int> CountJobsByCompanyIdAsync(int companyId);
        Task<int> CountReportsAgainstCompanyJobsAsync(int companyId);
        Task<int> CountReportsFiredByCandidateAsync(int candidateId);

        Task<ReportNotificationInfo?> GetReportNotificationInfoAsync(int reportId);
    }

    public class ReportNotificationInfo
    {
        public int CandidateId { get; set; }
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
    }
}
