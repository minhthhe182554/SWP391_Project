using Microsoft.EntityFrameworkCore;
using SWP391_Project.Models;

namespace SWP391_Project.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly EzJobDbContext _context;

        public AdminRepository(EzJobDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<int> GetTotalUsersAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<int> GetTotalCandidatesAsync()
        {
            return await _context.Candidates.CountAsync();
        }

        public async Task<int> GetTotalCompaniesAsync()
        {
            return await _context.Companies.CountAsync();
        }

        public async Task<int> GetTotalJobsAsync()
        {
            return await _context.Jobs.IgnoreQueryFilters().CountAsync();
        }

        public async Task<int> GetTotalApplicationsAsync()
        {
            return await _context.Applications.CountAsync();
        }

        public async Task<int> GetTotalReportsAsync()
        {
            return await _context.Reports.CountAsync();
        }

        public async Task<int> GetPendingReportsAsync()
        {
            return await _context.Reports.CountAsync(r => r.Status == ReportStatus.PENDING);
        }
    }
}
