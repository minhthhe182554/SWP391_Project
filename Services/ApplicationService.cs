using OfficeOpenXml.Style;
using OfficeOpenXml;
using SWP391_Project.Models;
using SWP391_Project.Repositories;
using SWP391_Project.ViewModels.Candidate;
using SWP391_Project.ViewModels.Company;
using SWP391_Project.Services.Storage;

namespace SWP391_Project.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly ICandidateRepository _candidateRepository;
        private readonly IJobRepository _jobRepository;
        private readonly IStorageService _storageService;

        public ApplicationService(
            IApplicationRepository applicationRepository,
            ICandidateRepository candidateRepository,
            IJobRepository jobRepository, IStorageService storageService)
        {
            _applicationRepository = applicationRepository;
            _candidateRepository = candidateRepository;
            _jobRepository = jobRepository;
            _storageService = storageService;
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
        public async Task<List<AppliedJobVM>> GetAppliedJobsAsync(int userId)
        {
            var candidate = await _candidateRepository.GetByUserIdAsync(userId);
            if (candidate == null) return new List<AppliedJobVM>();

            var applications = await _applicationRepository.GetApplicationsByCandidateIdAsync(candidate.Id);

            return applications.Select(a => new AppliedJobVM
            {
                JobId = a.JobId,
                JobTitle = a.Job.Title,
                CompanyName = a.Job.Company?.Name ?? "Công ty ẩn danh",
                CompanyLogo = a.Job.Company?.ImageUrl ?? "/imgs/ic_default_avatar.png", 
                Location = a.Job.Location?.City ?? "Chưa cập nhật",

                SalaryText = (a.Job.LowerSalaryRange.HasValue || a.Job.HigherSalaryRange.HasValue)
                             ? "Có lương" : "Thỏa thuận",

                AppliedDate = a.SentDate,
                ResumeUrl = a.ResumeUrl,
                CoverLetter = a.CoverLetter
            }).ToList();
        }
        public async Task<JobApplicantsVM> GetApplicantsForJobAsync(int companyId, int jobId)
        {
            var job = await _jobRepository.GetByIdAsync(jobId);
            if (job == null || job.CompanyId != companyId) throw new Exception("Job not found or unauthorized");

            var applications = await _applicationRepository.GetApplicationsByJobIdAsync(jobId);

            return new JobApplicantsVM
            {
                JobId = job.Id,
                JobTitle = job.Title,
                Applicants = applications.Select(a =>
                {
                    string publicId = ExtractPublicIdFromUrl(a.ResumeUrl);

                    var previewLinks = GeneratePreviewLinks(publicId, 3);

                    return new ApplicantDto
                    {
                        ApplicationId = a.Id,
                        CandidateId = a.CandidateId,
                        FullName = a.FullName,
                        Email = a.Email,
                        PhoneNumber = a.PhoneNumber,
                        CvUrl = a.ResumeUrl,
                        CoverLetter = a.CoverLetter,
                        AvatarUrl = a.Candidate?.ImageUrl, 
                        ApplyDate = a.SentDate,

                        PreviewUrls = previewLinks
                    };
                }).ToList()
            };
        }

        public async Task<byte[]> ExportApplicantsToExcelAsync(int companyId, int jobId)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var vm = await GetApplicantsForJobAsync(companyId, jobId);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Applicants");

                worksheet.Cells[1, 1].Value = "STT";
                worksheet.Cells[1, 2].Value = "Họ và Tên";
                worksheet.Cells[1, 3].Value = "Email";
                worksheet.Cells[1, 4].Value = "Số điện thoại";
                worksheet.Cells[1, 5].Value = "Ngày nộp";
                worksheet.Cells[1, 6].Value = "Link CV";
                worksheet.Cells[1, 7].Value = "Thư giới thiệu";

                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                }

                int row = 2;
                int stt = 1;
                foreach (var app in vm.Applicants)
                {
                    worksheet.Cells[row, 1].Value = stt++;
                    worksheet.Cells[row, 2].Value = app.FullName;
                    worksheet.Cells[row, 3].Value = app.Email;
                    worksheet.Cells[row, 4].Value = app.PhoneNumber;
                    worksheet.Cells[row, 5].Value = app.ApplyDate.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cells[row, 6].Value = app.CvUrl;
                    worksheet.Cells[row, 7].Value = app.CoverLetter;
                    row++;
                }
                worksheet.Cells.AutoFitColumns();

                return package.GetAsByteArray();
            }
        }
        private List<string> GeneratePreviewLinks(string publicId, int pageCount)
        {
            var urls = new List<string>();
            if (string.IsNullOrEmpty(publicId)) return urls;

            for (int i = 1; i <= pageCount; i++)
            {
                var url = _storageService.BuildPdfImageUrl(publicId, page: i, width: 900, density: 150);
                urls.Add(url);
            }
            return urls;
        }

        private string ExtractPublicIdFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return "";

            if (!url.StartsWith("http")) return url;

            try
            {
                var uri = new Uri(url);
                var path = uri.AbsolutePath;

                var parts = path.Split(new[] { "upload/" }, StringSplitOptions.None);
                if (parts.Length < 2) return "";

                var afterUpload = parts[1]; 

                var segments = afterUpload.Split('/');
                var publicIdWithExt = string.Join("/", segments.Skip(1)); 

                var lastDot = publicIdWithExt.LastIndexOf('.');
                if (lastDot > 0)
                {
                    return publicIdWithExt.Substring(0, lastDot); 
                }

                return publicIdWithExt;
            }
            catch
            {
                return "";
            }
        }
    }
}
