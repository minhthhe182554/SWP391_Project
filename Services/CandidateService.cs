using SWP391_Project.Models;
using SWP391_Project.Repositories;
using SWP391_Project.ViewModels;

namespace SWP391_Project.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly ICandidateRepository _candidateRepository;
        private readonly ILogger<CandidateService> _logger;

        public CandidateService(ICandidateRepository candidateRepository, ILogger<CandidateService> logger)
        {
            _candidateRepository = candidateRepository;
            _logger = logger;
        }

        public async Task<Candidate?> GetCandidateByUserIdAsync(int userId)
        {
            try
            {
                return await _candidateRepository.GetByUserIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting candidate by user id {UserId}", userId);
                throw;
            }
        }

        public async Task<CandidateHomeVM> GetCandidateHomeViewAsync(int userId)
        {
            try
            {
                var recommendedJobs = await _candidateRepository.GetRecommendedJobsAsync(10);
                var allJobs = await _candidateRepository.GetAllActiveJobsAsync();

                return new CandidateHomeVM
                {
                    RecommendedJobs = recommendedJobs,
                    AllJobs = allJobs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting candidate home view for user {UserId}", userId);
                throw;
            }
        }
    }
}
