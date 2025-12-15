using SWP391_Project.Models;
using System.Threading.Tasks;

namespace SWP391_Project.Repositories
{
    public interface IJobRepository
    {
        Task<Job?> GetJobWithDetailsAsync(int jobId);
        Task<List<Job>> GetActiveJobsWithDetailsAsync();
        Task<Job?> GetByIdAsync(int id);
        Task AddAsync(Job job);
    }
}

