using SWP391_Project.Models;

namespace SWP391_Project.Repositories
{
    public interface ICandidateRepository
    {
        Task<Candidate?> GetByUserIdAsync(int userId);
        Task<List<Job>> GetRecommendedJobsAsync(int limit = 10);
        Task<List<Job>> GetAllActiveJobsAsync();
    }
}
