using System.ComponentModel.DataAnnotations;

namespace SWP391_Project.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Vui long nhap email!")]
        [EmailAddress(ErrorMessage = "Email khong dung dinh dang")]
        [StringLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Vui long nhap mat khau")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có từ 6 đến 255 ký tự")]
        public string Password { get; set; } = null!;
    }
}
