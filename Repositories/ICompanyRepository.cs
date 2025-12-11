using SWP391_Project.Models;

namespace SWP391_Project.Repositories
{
    public interface ICompanyRepository
    {
        Task<Company?> GetByUserIdAsync(int userId);
        Task<int> GetTotalJobsAsync(int companyId);
        Task<int> GetActiveJobsAsync(int companyId);
        Task<int> GetTotalApplicationsAsync(int companyId);
        Task<Company?> GetByIdAsync(int id); 
        Task UpdateAsync(Company company);
        Task<Location> GetOrCreateLocationAsync(string city, string ward);
    }
}
