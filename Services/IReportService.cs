using SWP391_Project.Models.Enums;
using SWP391_Project.ViewModels.Candidate;

namespace SWP391_Project.Services
{
    public interface IReportService
    {
        Task<CandidateReportListVM> GetCandidateReportsAsync(int userId, ReportStatus? statusFilter);
        Task<CandidateReportDetailVM?> GetCandidateReportDetailAsync(int userId, int reportId);
        Task<(bool Success, string Message)> UpdateCandidateReportAsync(int userId, int reportId, string newReason);
        Task<(bool Success, string Message)> DeleteCandidateReportAsync(int userId, int reportId);
    }
}
