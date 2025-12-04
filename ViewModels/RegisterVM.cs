using SWP391_Project.Models;
using System.ComponentModel.DataAnnotations;

namespace SWP391_Project.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Vui long nhap mat khau")]
        [MinLength(6, ErrorMessage = "Mat khau phai co it nhat 6 ky tu")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Vui long xac nhan mat khau")]
        [Compare("Password", ErrorMessage = "Mat khau xac nhan khong khop")]
        public string ConfirmPassword { get; set; } = null!;

        [Required(ErrorMessage = "Vui long nhap ten hien thi")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Vui long chon vai tro cua ban")]
        public Role Role { get; set; }
    }
}
