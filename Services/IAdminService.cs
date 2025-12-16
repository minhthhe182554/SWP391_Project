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
            int pageSize);
        Task<ManageReportsVM> GetManageReportsAsync(int page, int pageSize);
        Task<UserDetailVM?> GetUserDetailAsync(int userId);
        Task ToggleUserActiveAsync(int userId, bool active);
    }
}
