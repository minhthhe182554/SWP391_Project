using System.ComponentModel.DataAnnotations;

namespace SWP391_Project.ViewModels.Company
{
    public class CompanyProfileVM
    {
        public int Id { get; set; }
        [Display(Name = "Tên công ty")]
        [Required(ErrorMessage = "Vui lòng nhập tên công ty")]
        public string Name { get; set; } = null!;
        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string PhoneNumber { get; set; } = null!;
        [Display(Name = "Website")]
        public string? Website { get; set; }

        [Display(Name = "Địa chỉ chi tiết")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ chi tiết")]
        public string Address { get; set; } = null!;
        [Display(Name = "Thành phố")]
        [Required(ErrorMessage = "Vui lòng chọn thành phố/tỉnh")]
        public string City { get; set; } = null!;

        [Display(Name = "Phường/Xã")]
        [Required(ErrorMessage = "Vui lòng chọn quận/huyện")]
        public string Ward { get; set; } = null!;

        [Display(Name = "Mô tả về công ty")]
        [Required(ErrorMessage = "Vui lòng nhập mô tả về công ty")]
        public string Description { get; set; } = null!;

        public string? ExistingImageUrl { get; set; }
        
        [Display(Name = "Logo công ty")]
        public IFormFile? AvatarFile { get; set; }
    }
}
