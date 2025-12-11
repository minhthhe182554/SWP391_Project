using SWP391_Project.Models;
using SWP391_Project.ViewModels;
using SWP391_Project.ViewModels.Company;

namespace SWP391_Project.Services
{
    public interface ICompanyService
    {
        Task<Company?> GetCompanyByUserIdAsync(int userId);
        Task<CompanyDashboardVM> GetCompanyDashboardViewAsync(int companyId);
        Task<CompanyProfileVM?> GetProfileForEditAsync(int userId);
        Task<bool> UpdateProfileAsync(int userId, CompanyProfileVM model);
    }
}
