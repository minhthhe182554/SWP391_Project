using SWP391_Project.Models;
using SWP391_Project.ViewModels;

namespace SWP391_Project.Services
{
    public interface ICandidateService
    {
        Task<Candidate?> GetCandidateByUserIdAsync(int userId);
        Task<CandidateHomeVM> GetCandidateHomeViewAsync(int userId);
    }
}
