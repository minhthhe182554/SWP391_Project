using SWP391_Project.Models;
using SWP391_Project.Repositories;
using SWP391_Project.ViewModels;
using SWP391_Project.ViewModels.Candidate;

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

        public async Task<CandidateProfileVM?> GetProfileAsync(int userId)
        {
            try
            {
                var candidate = await _candidateRepository.GetProfileByUserIdAsync(userId);
                if (candidate == null) return null;

                return new CandidateProfileVM
                {
                    CandidateId = candidate.Id,
                    FullName = candidate.FullName,
                    PhoneNumber = candidate.PhoneNumber,
                    Email = candidate.User.Email,
                    ImageUrl = candidate.ImageUrl,
                    Jobless = candidate.Jobless,
                    RemainingReport = candidate.RemainingReport,
                    EducationRecords = candidate.EducationRecords,
                    WorkExperiences = candidate.WorkExperiences,
                    Skills = candidate.Skills
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting profile for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> UpdateProfileAsync(int userId, string fullName, string? phoneNumber)
        {
            try
            {
                var candidate = await _candidateRepository.GetByUserIdAsync(userId);
                if (candidate == null) return false;

                candidate.FullName = fullName;
                candidate.PhoneNumber = phoneNumber;
                return await _candidateRepository.UpdateCandidateAsync(candidate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> UpdateJoblessStatusAsync(int userId, bool jobless)
        {
            try
            {
                var candidate = await _candidateRepository.GetByUserIdAsync(userId);
                if (candidate == null) return false;

                candidate.Jobless = jobless;
                return await _candidateRepository.UpdateCandidateAsync(candidate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating jobless status for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> UpdatePasswordAsync(int userId, string newPassword)
        {
            try
            {
                return await _candidateRepository.UpdateUserPasswordAsync(userId, newPassword);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating password for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> AddEducationRecordAsync(int candidateId, EducationRecord record)
        {
            try
            {
                record.CandidateId = candidateId;
                return await _candidateRepository.AddEducationRecordAsync(record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding education record for candidate {CandidateId}", candidateId);
                return false;
            }
        }

        public async Task<bool> DeleteEducationRecordAsync(int candidateId, int recordId)
        {
            try
            {
                return await _candidateRepository.DeleteEducationRecordAsync(recordId, candidateId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting education record {RecordId} for candidate {CandidateId}", recordId, candidateId);
                return false;
            }
        }

        public async Task<bool> AddWorkExperienceAsync(int candidateId, WorkExperience experience)
        {
            try
            {
                experience.CandidateId = candidateId;
                return await _candidateRepository.AddWorkExperienceAsync(experience);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding work experience for candidate {CandidateId}", candidateId);
                return false;
            }
        }

        public async Task<bool> DeleteWorkExperienceAsync(int candidateId, int experienceId)
        {
            try
            {
                return await _candidateRepository.DeleteWorkExperienceAsync(experienceId, candidateId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting work experience {ExperienceId} for candidate {CandidateId}", experienceId, candidateId);
                return false;
            }
        }

        public async Task<bool> AddSkillAsync(int candidateId, int skillId)
        {
            try
            {
                return await _candidateRepository.AddSkillToCandidateAsync(candidateId, skillId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding skill {SkillId} to candidate {CandidateId}", skillId, candidateId);
                return false;
            }
        }

        public async Task<bool> RemoveSkillAsync(int candidateId, int skillId)
        {
            try
            {
                return await _candidateRepository.RemoveSkillFromCandidateAsync(candidateId, skillId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing skill {SkillId} from candidate {CandidateId}", skillId, candidateId);
                return false;
            }
        }

        public async Task<List<Skill>> GetAllSkillsAsync()
        {
            try
            {
                return await _candidateRepository.GetAllSkillsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all skills");
                throw;
            }
        }

        public async Task<bool> CreateAndAddSkillAsync(int candidateId, string skillName)
        {
            try
            {
                return await _candidateRepository.CreateAndAddSkillAsync(candidateId, skillName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating and adding skill for candidate {CandidateId}", candidateId);
                return false;
            }
        }

        public async Task<bool> UpdateProfileImageAsync(int candidateId, string publicId)
        {
            try
            {
                return await _candidateRepository.UpdateProfileImageAsync(candidateId, publicId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile image for candidate {CandidateId}", candidateId);
                return false;
            }
        }
    }
}
