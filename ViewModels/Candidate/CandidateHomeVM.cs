using SWP391_Project.Models;

namespace SWP391_Project.ViewModels
{
    public class CandidateHomeVM
    {
        public List<Job> RecommendedJobs { get; set; } = new();
        public List<Job> AllJobs { get; set; } = new();
    }
}
