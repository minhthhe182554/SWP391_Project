using SWP391_Project.Dtos;
using SWP391_Project.Models;
using System.ComponentModel.DataAnnotations;

namespace SWP391_Project.ViewModels.Company
{
    public class PostJobVM
    {
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn địa điểm")]
        public int LocationId { get; set; }
        public string? CityCode { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Tỉnh/Thành phố")]
        public string CityName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn Phường/Xã")]
        public string WardName { get; set; } = null!;

        public string? Address { get; set; }
        public decimal? LowerSalary { get; set; }
        public decimal? HigherSalary { get; set; }

        [Required(ErrorMessage = "Nhập số năm kinh nghiệm")]
        public int YearsOfExperience { get; set; }

        [Required(ErrorMessage = "Nhập mô tả công việc")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Chọn hạn nộp hồ sơ")]
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(30);

        [Required(ErrorMessage = "Chọn loại hình công việc")]
        public JobType JobType { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ít nhất 1 kỹ năng")]
        public string SelectedSkills { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập ít nhất 1 lĩnh vực")]
        public string SelectedDomains { get; set; } = null!;

        [Required(ErrorMessage = "Nhập số lượng tuyển")]
        [Range(1, 100, ErrorMessage = "Số lượng phải từ 1 đến 100")]
        [Display(Name = "Số lượng tuyển")]
        public int VacancyCount { get; set; } = 1;
        public List<CityDto> Cities { get; set; } = new();
        public List<Location> Locations { get; set; } = new();
        public List<Domain> Domains { get; set; } = new();
        public List<Skill> Skills { get; set; } = new();
    }
}
