using SWP391_Project.Models;

namespace SWP391_Project.ViewModels
{
    public class CandidateHomeVM
    {
        public List<Models.Job> RecommendedJobs { get; set; } = new();
        public List<Models.Job> AllJobs { get; set; } = new();
    }
}
