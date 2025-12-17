using Microsoft.EntityFrameworkCore;
using SWP391_Project.Models;

namespace SWP391_Project.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly EzJobDbContext _context;

        public ApplicationRepository(EzJobDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasAppliedAsync(int candidateId, int jobId)
        {
            return await _context.Applications
                .AnyAsync(a => a.CandidateId == candidateId && a.JobId == jobId);
        }

        public async Task AddAsync(Application application)
        {
            _context.Applications.Add(application);
            await _context.SaveChangesAsync();
        }

        public async Task<Resume?> GetResumeByIdAsync(int resumeId)
        {
            return await _context.Resumes.FindAsync(resumeId);
        }

        public async Task<List<Resume>> GetResumesByCandidateIdAsync(int candidateId)
        {
            return await _context.Resumes
                .Where(r => r.CandidateId == candidateId)
                .ToListAsync();
        }
        public async Task<Application?> GetApplicationAsync(int candidateId, int jobId)
        {
            return await _context.Applications
                .FirstOrDefaultAsync(a => a.CandidateId == candidateId && a.JobId == jobId);
        }

        public async Task UpdateAsync(Application application)
        {
            _context.Applications.Update(application);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Application>> GetApplicationsByCandidateIdAsync(int candidateId)
        {
            return await _context.Applications
                .Where(a => a.CandidateId == candidateId)
                .Include(a => a.Job)                
                    .ThenInclude(j => j.Company)    
                .Include(a => a.Job)
                    .ThenInclude(j => j.Location)   
                .OrderByDescending(a => a.SentDate) 
                .ToListAsync();
        }

        public async Task<List<Application>> GetApplicationsByJobIdAsync(int jobId)
        {
            return await _context.Applications
        .Where(a => a.JobId == jobId)
        .Include(a => a.Candidate)
        .OrderByDescending(a => a.SentDate)
        .ToListAsync();
        }
    }
}
