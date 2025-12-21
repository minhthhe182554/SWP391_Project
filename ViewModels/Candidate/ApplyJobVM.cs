using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SWP391_Project.Models;
using System.ComponentModel.DataAnnotations;

namespace SWP391_Project.ViewModels.Candidate
{
    public class ApplyJobVM
    {
        public int JobId { get; set; }
        [ValidateNever]
        public string JobTitle { get; set; } = null!;
        [ValidateNever]
        public string CompanyLogo { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn CV để ứng tuyển")]
        public int SelectedResumeId { get; set; }

        public string? CoverLetter { get; set; }
        [ValidateNever]
        public List<Resume> ExistingResumes { get; set; } = new();
    }
}
