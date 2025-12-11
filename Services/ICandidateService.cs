using SWP391_Project.Models;
using SWP391_Project.ViewModels;
using SWP391_Project.ViewModels.Candidate;

namespace SWP391_Project.Services
{
    public interface ICandidateService
    {
        Task<Candidate?> GetCandidateByUserIdAsync(int userId);
        Task<CandidateHomeVM> GetCandidateHomeViewAsync(int userId);
        Task<CandidateProfileVM?> GetProfileAsync(int userId);
        Task<bool> UpdateProfileAsync(int userId, string fullName, string? phoneNumber);
        Task<bool> UpdateJoblessStatusAsync(int userId, bool jobless);
        Task<bool> UpdatePasswordAsync(int userId, string newPassword);
        Task<bool> AddEducationRecordAsync(int candidateId, EducationRecord record);
        Task<bool> DeleteEducationRecordAsync(int candidateId, int recordId);
        Task<bool> AddWorkExperienceAsync(int candidateId, WorkExperience experience);
        Task<bool> DeleteWorkExperienceAsync(int candidateId, int experienceId);
        Task<bool> AddSkillAsync(int candidateId, int skillId);
        Task<bool> RemoveSkillAsync(int candidateId, int skillId);
        Task<List<Skill>> GetAllSkillsAsync();
        Task<bool> CreateAndAddSkillAsync(int candidateId, string skillName);
        Task<bool> UpdateProfileImageAsync(int candidateId, string publicId);
        Task<CandidateResumeVM?> GetResumesAsync(int userId);
        Task<(bool Success, string Message)> UploadResumeAsync(int userId, string? name, IFormFile resumeFile);
        Task<bool> DeleteResumeAsync(int userId, int resumeId);
        Task<(bool Success, string Message)> UpdateResumeNameAsync(int userId, int resumeId, string newName);
        Task<(bool Success, string Message)> UpdateResumeFileAsync(int userId, int resumeId, IFormFile resumeFile);
        Task<bool> AddCertificateAsync(int candidateId, Certificate certificate);
        Task<bool> DeleteCertificateAsync(int candidateId, int certificateId);
    }
}
