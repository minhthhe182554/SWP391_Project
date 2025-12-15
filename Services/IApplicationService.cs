using SWP391_Project.ViewModels;

namespace SWP391_Project.Services
{
    public interface IApplicationService
    {
        Task<ApplyJobVM> GetApplyFormAsync(int userId, int jobId);
        Task<(bool success, string message)> SubmitApplicationAsync(int userId, ApplyJobVM model);
    }
}
