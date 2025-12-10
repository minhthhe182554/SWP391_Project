using SWP391_Project.Models;

namespace SWP391_Project.Repositories
{
    public interface ICompanyRepository
    {
        Task<Company?> GetByUserIdAsync(int userId);
        Task<int> GetTotalJobsAsync(int companyId);
        Task<int> GetActiveJobsAsync(int companyId);
        Task<int> GetTotalApplicationsAsync(int companyId);
    }
}
