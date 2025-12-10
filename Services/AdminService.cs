using SWP391_Project.Models;
using SWP391_Project.Repositories;
using SWP391_Project.ViewModels;

namespace SWP391_Project.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ILogger<AdminService> _logger;

        public AdminService(IAdminRepository adminRepository, ILogger<AdminService> logger)
        {
            _adminRepository = adminRepository;
            _logger = logger;
        }

        public async Task<User?> GetAdminUserByIdAsync(int userId)
        {
            try
            {
                return await _adminRepository.GetUserByIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin user by id {UserId}", userId);
                throw;
            }
        }

        public async Task<AdminDashboardVM> GetAdminDashboardViewAsync()
        {
            try
            {
                var totalUsers = await _adminRepository.GetTotalUsersAsync();
                var totalCandidates = await _adminRepository.GetTotalCandidatesAsync();
                var totalCompanies = await _adminRepository.GetTotalCompaniesAsync();
                var totalJobs = await _adminRepository.GetTotalJobsAsync();
                var totalApplications = await _adminRepository.GetTotalApplicationsAsync();
                var totalReports = await _adminRepository.GetTotalReportsAsync();
                var pendingReports = await _adminRepository.GetPendingReportsAsync();

                return new AdminDashboardVM
                {
                    TotalUsers = totalUsers,
                    TotalCandidates = totalCandidates,
                    TotalCompanies = totalCompanies,
                    TotalJobs = totalJobs,
                    TotalApplications = totalApplications,
                    TotalReports = totalReports,
                    PendingReports = pendingReports
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin dashboard view");
                throw;
            }
        }
    }
}
