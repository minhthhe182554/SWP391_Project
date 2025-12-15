using SWP391_Project.Models;
using JobEntity = SWP391_Project.Models.Job; 

namespace SWP391_Project.ViewModels
{
    public class CandidateHomeVM
    {
        public List<JobEntity> RecommendedJobs { get; set; } = new();
        public List<JobEntity> AllJobs { get; set; } = new();
    }
}