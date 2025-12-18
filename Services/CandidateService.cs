using SWP391_Project.Models;
using SWP391_Project.Repositories;
using SWP391_Project.ViewModels;
using SWP391_Project.ViewModels.Candidate;
using System.Text.RegularExpressions;
using CloudinaryDotNet.Actions;
using SWP391_Project.Services.Storage;

namespace SWP391_Project.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly ICandidateRepository _candidateRepository;
        private readonly ILogger<CandidateService> _logger;
        private readonly IStorageService _storageService;

        public CandidateService(ICandidateRepository candidateRepository, ILogger<CandidateService> logger, IStorageService storageService)
        {
            _candidateRepository = candidateRepository;
            _logger = logger;
            _storageService = storageService;
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
                    Certificates = candidate.Certificates,
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

        public async Task<CandidateResumeVM?> GetResumesAsync(int userId)
        {
            try
            {
                var candidate = await _candidateRepository.GetCandidateWithResumesByUserIdAsync(userId);
                if (candidate == null) return null;

                return new CandidateResumeVM
                {
                    CandidateId = candidate.Id,
                    Resumes = candidate.Resumes
                        .OrderByDescending(r => r.Id)
                        .Select(r => new CandidateResumeItemVM
                        {
                            Id = r.Id,
                            Name = string.IsNullOrWhiteSpace(r.Name) ? "Không tên" : r.Name,
                            Url = _storageService.BuildRawUrl(r.Url),
                            OriginalFileName = ExtractOriginalFileName(r.Url),
                            PreviewUrls = Enumerable.Range(1, 3)
                                .Select(pg => _storageService.BuildPdfImageUrl(r.Url, pg, 900, 150))
                                .ToList()
                        })
                        .ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting resumes for user {UserId}", userId);
                throw;
            }
        }

        public async Task<(bool Success, string Message)> UploadResumeAsync(int userId, string? name, IFormFile resumeFile)
        {
            try
            {
                var candidate = await _candidateRepository.GetCandidateWithResumesByUserIdAsync(userId);
                if (candidate == null)
                {
                    return (false, "Không tìm thấy ứng viên");
                }

                if (string.IsNullOrWhiteSpace(name))
                {
                    return (false, "Vui lòng nhập tên hồ sơ");
                }

                if (resumeFile == null || resumeFile.Length == 0)
                {
                    return (false, "Vui lòng chọn tệp PDF");
                }

                var extension = Path.GetExtension(resumeFile.FileName).ToLowerInvariant();
                if (extension != ".pdf")
                {
                    return (false, "Chỉ chấp nhận tệp PDF");
                }

                var originalFileName = Path.GetFileName(resumeFile.FileName);
                var baseName = Path.GetFileNameWithoutExtension(originalFileName);
                var sanitizedBase = SanitizeFileName(baseName);
                var publicId = sanitizedBase;
                var folder = $"resumes/{userId}";

                var uploadResult = await _storageService.UploadPdfAsync(resumeFile, folder, publicId);

                Console.WriteLine($"Resume info: {folder}, publicId: {publicId}" );
                var resume = new Resume
                {
                    Name = name.Trim(),
                    Url = uploadResult.PublicId, // storing publicId as Url field
                    CandidateId = candidate.Id
                };

                var saved = await _candidateRepository.AddResumeAsync(resume);
                if (!saved)
                {
                    // best-effort cleanup on cloudinary
                    await _storageService.DeleteAssetAsync(uploadResult.PublicId, ResourceType.Raw);
                    return (false, "Không lưu được hồ sơ. Vui lòng thử lại.");
                }

                return (true, "Tải lên thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading resume for user {UserId}", userId);
                return (false, "Có lỗi xảy ra khi tải lên");
            }
        }

        public async Task<bool> DeleteResumeAsync(int userId, int resumeId)
        {
            try
            {
                var candidate = await _candidateRepository.GetCandidateWithResumesByUserIdAsync(userId);
                if (candidate == null) return false;

                var resume = await _candidateRepository.GetResumeAsync(resumeId, candidate.Id);
                if (resume == null) return false;

                var deletedCloud = await _storageService.DeleteAssetAsync(resume.Url, ResourceType.Image);
                var deletedDb = await _candidateRepository.DeleteResumeAsync(resumeId, candidate.Id);
                return deletedDb && deletedCloud;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting resume {ResumeId} for user {UserId}", resumeId, userId);
                return false;
            }
        }

        public async Task<(bool Success, string Message)> UpdateResumeNameAsync(int userId, int resumeId, string newName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(newName))
                {
                    return (false, "Tên hồ sơ không được để trống");
                }

                var candidate = await _candidateRepository.GetCandidateWithResumesByUserIdAsync(userId);
                if (candidate == null) return (false, "Không tìm thấy ứng viên");

                var resume = await _candidateRepository.GetResumeAsync(resumeId, candidate.Id);
                if (resume == null) return (false, "Không tìm thấy hồ sơ");

                resume.Name = newName.Trim();
                var updated = await _candidateRepository.UpdateResumeAsync(resume);
                return updated ? (true, "Đã cập nhật tên hồ sơ") : (false, "Cập nhật thất bại");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating resume name {ResumeId} for user {UserId}", resumeId, userId);
                return (false, "Có lỗi xảy ra khi cập nhật tên");
            }
        }

        public async Task<(bool Success, string Message)> UpdateResumeFileAsync(int userId, int resumeId, IFormFile resumeFile)
        {
            try
            {
                if (resumeFile == null || resumeFile.Length == 0)
                {
                    return (false, "Vui lòng chọn tệp PDF");
                }

                var extension = Path.GetExtension(resumeFile.FileName).ToLowerInvariant();
                if (extension != ".pdf")
                {
                    return (false, "Chỉ chấp nhận tệp PDF");
                }

                var candidate = await _candidateRepository.GetCandidateWithResumesByUserIdAsync(userId);
                if (candidate == null) return (false, "Không tìm thấy ứng viên");

                var resume = await _candidateRepository.GetResumeAsync(resumeId, candidate.Id);
                if (resume == null) return (false, "Không tìm thấy hồ sơ");

                // delete old on cloud
                await _storageService.DeleteAssetAsync(resume.Url, ResourceType.Image); 

                var originalFileName = Path.GetFileName(resumeFile.FileName);
                var baseName = Path.GetFileNameWithoutExtension(originalFileName);
                var sanitizedBase = SanitizeFileName(baseName);
                var publicId = sanitizedBase;
                var folder = $"resumes/{userId}";

                var uploadResult = await _storageService.UploadPdfAsync(resumeFile, folder, publicId);

                resume.Url = uploadResult.PublicId;
                var updated = await _candidateRepository.UpdateResumeAsync(resume);
                return updated ? (true, "Đã cập nhật hồ sơ") : (false, "Cập nhật thất bại");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating resume file {ResumeId} for user {UserId}", resumeId, userId);
                return (false, "Có lỗi xảy ra khi cập nhật file");
            }
        }

        private static string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var cleaned = string.Concat(fileName.Where(ch => !invalidChars.Contains(ch)));
            cleaned = Regex.Replace(cleaned, @"\s+", "-");
            return string.IsNullOrWhiteSpace(cleaned) ? "resume" : cleaned;
        }

        private static string ExtractOriginalFileName(string publicId)
        {
            if (string.IsNullOrEmpty(publicId)) return publicId;

            // Take last segment after folder and append pdf for display
            var fileName = Path.GetFileName(publicId);
            return fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
                ? fileName
                : $"{fileName}.pdf";
        }

        public async Task<bool> AddCertificateAsync(int candidateId, Certificate certificate)
        {
            try
            {
                certificate.CandidateId = candidateId;
                return await _candidateRepository.AddCertificateAsync(certificate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding certificate for candidate {CandidateId}", candidateId);
                return false;
            }
        }

        public async Task<bool> DeleteCertificateAsync(int candidateId, int certificateId)
        {
            try
            {
                return await _candidateRepository.DeleteCertificateAsync(certificateId, candidateId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting certificate {CertificateId} for candidate {CandidateId}", certificateId, candidateId);
                return false;
            }
        }
    }
}
