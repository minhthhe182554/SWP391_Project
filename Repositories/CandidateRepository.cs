using Microsoft.EntityFrameworkCore;
using SWP391_Project.Models;

namespace SWP391_Project.Repositories
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly EzJobDbContext _context;

        public CandidateRepository(EzJobDbContext context)
        {
            _context = context;
        }

        public async Task<Candidate?> GetByUserIdAsync(int userId)
        {
            return await _context.Candidates
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<List<Job>> GetRecommendedJobsAsync(int limit = 10)
        {
            return await _context.Jobs
                .Include(j => j.Company)
                .Include(j => j.Location)
                .Include(j => j.RequiredSkills)
                .Where(j => j.EndDate >= DateTime.Now && !j.IsDelete)
                .OrderByDescending(j => j.StartDate)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<Job>> GetAllActiveJobsAsync()
        {
            return await _context.Jobs
                .Include(j => j.Company)
                .Include(j => j.Location)
                .Include(j => j.RequiredSkills)
                .Where(j => j.EndDate >= DateTime.Now && !j.IsDelete)
                .OrderByDescending(j => j.StartDate)
                .ToListAsync();
        }
    }
}
