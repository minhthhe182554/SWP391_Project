using Microsoft.EntityFrameworkCore;
using SWP391_Project.Models;

namespace SWP391_Project.Repositories
{
    public class SavedJobRepository : ISavedJobRepository
    {
        private readonly EzJobDbContext _context;

        public SavedJobRepository(EzJobDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsSavedAsync(int candidateId, int jobId)
        {
            return await _context.SavedJobs
                .AnyAsync(sj => sj.CandidateId == candidateId && sj.JobId == jobId);
        }

        public async Task AddAsync(SavedJob savedJob)
        {
            _context.SavedJobs.Add(savedJob);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(SavedJob savedJob)
        {
            _context.SavedJobs.Remove(savedJob);
            await _context.SaveChangesAsync();
        }

        public async Task<SavedJob?> GetAsync(int candidateId, int jobId)
        {
            return await _context.SavedJobs
                .FirstOrDefaultAsync(sj => sj.CandidateId == candidateId && sj.JobId == jobId);
        }

        public async Task<List<Job>> GetSavedJobsByCandidateIdAsync(int candidateId)
        {
            return await _context.SavedJobs
                .Where(sj => sj.CandidateId == candidateId)
                .Include(sj => sj.Job)
                    .ThenInclude(j => j.Company)
                .Include(sj => sj.Job)
                    .ThenInclude(j => j.Location)
                .Select(sj => sj.Job)
                .ToListAsync();
        }
    }
}
