using SWP391_Project.Models;
using SWP391_Project.Repositories;
using SWP391_Project.ViewModels;

namespace SWP391_Project.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly ICandidateRepository _candidateRepository;
        private readonly IJobRepository _jobRepository;

        public ApplicationService(
            IApplicationRepository applicationRepository,
            ICandidateRepository candidateRepository,
            IJobRepository jobRepository)
        {
            _applicationRepository = applicationRepository;
            _candidateRepository = candidateRepository;
            _jobRepository = jobRepository;
        }

        public async Task<ApplyJobVM> GetApplyFormAsync(int userId, int jobId)
        {
            var candidate = await _candidateRepository.GetByUserIdAsync(userId);
            if (candidate == null) throw new Exception("Candidate not found");

            var job = await _jobRepository.GetByIdAsync(jobId);
            if (job == null) throw new Exception("Job not found");

            var resumes = await _applicationRepository.GetResumesByCandidateIdAsync(candidate.Id);

            return new ApplyJobVM
            {
                JobId = job.Id,
                JobTitle = job.Title,
                CompanyLogo = job.Company?.ImageUrl ?? "default.png",
                FullName = candidate.FullName,
                Email = candidate.User.Email ?? "",
                PhoneNumber = candidate.PhoneNumber ?? "",
                ExistingResumes = resumes
            };
        }

        public async Task<(bool success, string message)> SubmitApplicationAsync(int userId, ApplyJobVM model)
        {
            var candidate = await _candidateRepository.GetByUserIdAsync(userId);
            if (candidate == null) return (false, "Không tìm thấy thông tin ứng viên.");

            var resume = await _applicationRepository.GetResumeByIdAsync(model.SelectedResumeId);
            if (resume == null || resume.CandidateId != candidate.Id)
            {
                return (false, "CV được chọn không hợp lệ.");
            }

            var existingApp = await _applicationRepository.GetApplicationAsync(candidate.Id, model.JobId);

            if (existingApp != null)
            {

                existingApp.FullName = model.FullName;
                existingApp.Email = model.Email;
                existingApp.PhoneNumber = model.PhoneNumber;
                existingApp.CoverLetter = model.CoverLetter;
                existingApp.ResumeUrl = resume.Url; 
                existingApp.SentDate = DateTime.Now; 

                await _applicationRepository.UpdateAsync(existingApp);

                return (true, "Cập nhật hồ sơ ứng tuyển thành công!");
            }
            else
            {
                var application = new Application
                {
                    CandidateId = candidate.Id,
                    JobId = model.JobId,
                    FullName = model.FullName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    ResumeUrl = resume.Url,
                    CoverLetter = model.CoverLetter,
                    SentDate = DateTime.Now
                };

                await _applicationRepository.AddAsync(application);
                return (true, "Ứng tuyển thành công!");
            }
        }
    }
}
