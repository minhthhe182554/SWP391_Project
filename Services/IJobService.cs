using SWP391_Project.ViewModels.Company;
using SWP391_Project.ViewModels.Jobs;
using SWP391_Project.ViewModels.Search;
using System.Threading.Tasks;

namespace SWP391_Project.Services
{
    public interface IJobService
    {
        Task<JobDetailVM> GetJobDetailAsync(int jobId, int? userId = null);
        Task<PostJobVM> GetPostJobModelAsync();
        Task AddJobAsync(int userId, PostJobVM model);
        Task<SearchPageVM> SearchJobsAsync(SearchFilter filter);
    }
}

