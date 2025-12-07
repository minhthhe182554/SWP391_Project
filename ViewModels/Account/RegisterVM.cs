using SWP391_Project.Models;
using System.ComponentModel.DataAnnotations;

namespace SWP391_Project.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập tên hiển thị")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn vai trò của bạn")]
        public Role Role { get; set; }

        // Company fields - chỉ required khi Role là COMPANY
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Ward { get; set; }
    }
}
