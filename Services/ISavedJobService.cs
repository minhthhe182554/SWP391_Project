using SWP391_Project.ViewModels.Job;

namespace SWP391_Project.Services
{
    public interface ISavedJobService
    {
        Task<bool> ToggleSaveJobAsync(int userId, int jobId); 
        Task<List<SavedJobVM>> GetSavedJobsAsync(int userId);
    }
}
