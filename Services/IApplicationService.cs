using SWP391_Project.ViewModels.Candidate;
using SWP391_Project.ViewModels.Company;

namespace SWP391_Project.Services
{
    public interface IApplicationService
    {
        Task<ApplyJobVM> GetApplyFormAsync(int userId, int jobId);
        Task<(bool success, string message)> SubmitApplicationAsync(int userId, ApplyJobVM model);
        Task<List<AppliedJobVM>> GetAppliedJobsAsync(int userId);
        Task<JobApplicantsVM> GetApplicantsForJobAsync(int companyId, int jobId);
        Task<byte[]> ExportApplicantsToExcelAsync(int companyId, int jobId);
    }
}
