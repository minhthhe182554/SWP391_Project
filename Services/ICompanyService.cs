using SWP391_Project.Models;
using SWP391_Project.ViewModels;

namespace SWP391_Project.Services
{
    public interface ICompanyService
    {
        Task<Company?> GetCompanyByUserIdAsync(int userId);
        Task<CompanyDashboardVM> GetCompanyDashboardViewAsync(int companyId);
    }
}
