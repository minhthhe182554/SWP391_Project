using SWP391_Project.Models;

namespace SWP391_Project.Repositories
{
    public interface IApplicationRepository
    {
        Task<bool> HasAppliedAsync(int candidateId, int jobId);
        Task AddAsync(Application application);
        Task<Resume?> GetResumeByIdAsync(int resumeId);

        Task<List<Resume>> GetResumesByCandidateIdAsync(int candidateId);
        Task<Application?> GetApplicationAsync(int candidateId, int jobId); 
        Task UpdateAsync(Application application);
    }
}
