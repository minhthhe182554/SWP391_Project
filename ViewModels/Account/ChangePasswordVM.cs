using System.ComponentModel.DataAnnotations;

namespace SWP391_Project.ViewModels.Account
{
    public class ChangePasswordVM
    {
        [Required(ErrorMessage = "Vui long nhap mat khau cu")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = null!;
        [Required(ErrorMessage = "Vui long nhap mat khau moi")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mat khau phai co it nhat 6 ky tu")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Vui long xac nhan mat khau moi")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
