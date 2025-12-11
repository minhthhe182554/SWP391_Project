using SWP391_Project.Models;

namespace SWP391_Project.Repositories
{
    public interface ICandidateRepository
    {
        Task<Candidate?> GetByUserIdAsync(int userId);
        Task<Candidate?> GetProfileByUserIdAsync(int userId);
        Task<List<Job>> GetRecommendedJobsAsync(int limit = 10);
        Task<List<Job>> GetAllActiveJobsAsync();
        Task<bool> UpdateCandidateAsync(Candidate candidate);
        Task<bool> UpdateUserPasswordAsync(int userId, string newPassword);
        Task<bool> AddEducationRecordAsync(EducationRecord record);
        Task<bool> DeleteEducationRecordAsync(int recordId, int candidateId);
        Task<bool> AddWorkExperienceAsync(WorkExperience experience);
        Task<bool> DeleteWorkExperienceAsync(int experienceId, int candidateId);
        Task<bool> AddSkillToCandidateAsync(int candidateId, int skillId);
        Task<bool> RemoveSkillFromCandidateAsync(int candidateId, int skillId);
        Task<List<Skill>> GetAllSkillsAsync();
        Task<bool> CreateAndAddSkillAsync(int candidateId, string skillName);
        Task<bool> UpdateProfileImageAsync(int candidateId, string publicId);
    }
}
