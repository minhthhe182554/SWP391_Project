using SWP391_Project.Models;

namespace SWP391_Project.Repositories
{
    public interface IAdminRepository
    {
        Task<User?> GetUserByIdAsync(int userId);
        Task<int> GetTotalUsersAsync();
        Task<int> GetTotalCandidatesAsync();
        Task<int> GetTotalCompaniesAsync();
        Task<int> GetTotalJobsAsync();
        Task<int> GetTotalApplicationsAsync();
        Task<int> GetTotalReportsAsync();
        Task<int> GetPendingReportsAsync();
    }
}
