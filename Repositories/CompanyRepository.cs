using Microsoft.EntityFrameworkCore;
using SWP391_Project.Models;

namespace SWP391_Project.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly EzJobDbContext _context;

        public CompanyRepository(EzJobDbContext context)
        {
            _context = context;
        }

        public async Task<Company?> GetByUserIdAsync(int userId)
        {
            return await _context.Companies
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<int> GetTotalJobsAsync(int companyId)
        {
            return await _context.Jobs.CountAsync(j => j.CompanyId == companyId && !j.IsDelete);
        }

        public async Task<int> GetActiveJobsAsync(int companyId)
        {
            return await _context.Jobs.CountAsync(j => 
                j.CompanyId == companyId && 
                j.EndDate >= DateTime.Now && 
                !j.IsDelete);
        }

        public async Task<int> GetTotalApplicationsAsync(int companyId)
        {
            return await _context.Applications
                .CountAsync(a => _context.Jobs.Any(j => 
                    j.CompanyId == companyId && 
                    j.Id == a.JobId));
        }
    }
}
