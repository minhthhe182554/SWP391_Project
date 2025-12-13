using Microsoft.EntityFrameworkCore;
using SWP391_Project.Models;

namespace SWP391_Project.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly EzJobDbContext _context;

        public JobRepository(EzJobDbContext context)
        {
            _context = context;
        }

        public async Task<Job?> GetJobWithDetailsAsync(int jobId)
        {
            return await _context.Jobs
                .Include(j => j.Company)
                .ThenInclude(c => c.Location)
                .Include(j => j.Location)
                .Include(j => j.RequiredSkills)
                .Include(j => j.Domains)
                .FirstOrDefaultAsync(j => j.Id == jobId && !j.IsDelete);
        }

        public async Task<List<Job>> GetActiveJobsWithDetailsAsync()
        {
            return await _context.Jobs
                .Where(j => !j.IsDelete)
                .Include(j => j.Company)
                    .ThenInclude(c => c.Location)
                .Include(j => j.Location)
                .Include(j => j.RequiredSkills)
                .Include(j => j.Domains)
                .ToListAsync();
        }
    }
}

