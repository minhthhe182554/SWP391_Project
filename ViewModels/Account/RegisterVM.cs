using SWP391_Project.Models;
using System.ComponentModel.DataAnnotations;

namespace SWP391_Project.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có từ 6 đến 255 ký tự")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có từ 6 đến 255 ký tự")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập tên hiển thị")]
        [StringLength(255, ErrorMessage = "Tên hiển thị không được vượt quá 255 ký tự")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn vai trò của bạn")]
        public Role Role { get; set; }

        // Company fields 
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string? PhoneNumber { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập mô tả công ty")]
        [StringLength(500, ErrorMessage = "Mô tả công ty không được vượt quá 500 ký tự")]
        public string? Description { get; set; }

        [StringLength(500, ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tỉnh/thành phố")]    
        [StringLength(100, ErrorMessage = "Thành phố không được vượt quá 100 ký tự")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn quận/huyện")]
        [StringLength(100, ErrorMessage = "Phường/Xã không được vượt quá 100 ký tự")]
        public string? Ward { get; set; }
    }
}
