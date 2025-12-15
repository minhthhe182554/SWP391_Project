using SWP391_Project.Models;
using SWP391_Project.Repositories;
using SWP391_Project.ViewModels.Job;

namespace SWP391_Project.Services
{
    public class SavedJobService : ISavedJobService
    {
        private readonly ISavedJobRepository _savedJobRepository;
        private readonly ICandidateRepository _candidateRepository; 

        public SavedJobService(ISavedJobRepository savedJobRepository, ICandidateRepository candidateRepository)
        {
            _savedJobRepository = savedJobRepository;
            _candidateRepository = candidateRepository;
        }

        public async Task<bool> ToggleSaveJobAsync(int userId, int jobId)
        {
            var candidate = await _candidateRepository.GetByUserIdAsync(userId);
            if (candidate == null) throw new Exception("User is not a candidate");

            var existingSave = await _savedJobRepository.GetAsync(candidate.Id, jobId);

            if (existingSave != null)
            {
                await _savedJobRepository.RemoveAsync(existingSave);
                return false; 
            }
            else
            {
                var newSave = new SavedJob
                {
                    CandidateId = candidate.Id,
                    JobId = jobId,
                };
                await _savedJobRepository.AddAsync(newSave);
                return true; 
            }
        }

        public async Task<List<SavedJobVM>> GetSavedJobsAsync(int userId)
        {
            var candidate = await _candidateRepository.GetByUserIdAsync(userId);
            if (candidate == null) return new List<SavedJobVM>();

            var jobs = await _savedJobRepository.GetSavedJobsByCandidateIdAsync(candidate.Id);

            return jobs.Select(j => new SavedJobVM
            {
                JobId = j.Id,
                Title = j.Title,
                CompanyName = j.Company.Name,
                CompanyLogo = j.Company.ImageUrl,
                Location = j.Location.City, 
                SalaryRange = "Thỏa thuận", 
                Deadline = j.EndDate,
                IsExpired = j.EndDate < DateTime.Now,
                IsExpiringSoon = j.EndDate > DateTime.Now && j.EndDate <= DateTime.Now.AddDays(3) 
            }).ToList();
        }
    }
}
