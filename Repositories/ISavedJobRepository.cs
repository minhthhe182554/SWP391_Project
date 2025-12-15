using SWP391_Project.Models;


namespace SWP391_Project.Repositories
{
    public interface ISavedJobRepository
    {
        Task<bool> IsSavedAsync(int candidateId, int jobId);
        Task AddAsync(SavedJob savedJob);
        Task RemoveAsync(SavedJob savedJob);
        Task<SavedJob?> GetAsync(int candidateId, int jobId);
        Task<List<Job>> GetSavedJobsByCandidateIdAsync(int candidateId);
    }
}
