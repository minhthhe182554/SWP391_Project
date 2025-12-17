using SWP391_Project.ViewModels.Company;
using SWP391_Project.ViewModels.Job;
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
        Task<List<ManageJobsVM>> GetCompanyJobsAsync(int userId);
        Task RepostJobAsync(int userId, int jobId);
        Task<bool> CanEditJobAsync(int jobId);
        Task StopRecruitmentAsync(int userId, int jobId);
        Task<SearchPageVM> SearchJobsAsync(SearchFilter filter);
    }
}

