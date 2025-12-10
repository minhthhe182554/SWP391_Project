using SWP391_Project.Models;
using SWP391_Project.ViewModels;

namespace SWP391_Project.Services
{
    public interface IAdminService
    {
        Task<User?> GetAdminUserByIdAsync(int userId);
        Task<AdminDashboardVM> GetAdminDashboardViewAsync();
    }
}
