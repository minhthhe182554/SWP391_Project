using System.ComponentModel.DataAnnotations;

namespace SWP391_Project.ViewModels
{
    public class ResetPasswordVM
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string Email { get; set; }


        [Required(ErrorMessage = "Vui long nhap mat khau moi")]
        [MinLength(6, ErrorMessage = "Mat khau phai co it nhat 6 ky tu")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Vui long xac nhan mat khau")]
        [Compare("NewPassword", ErrorMessage = "Mat khau xac nhan khong chinh xac")]
        public string ConfirmPassword { get; set;} 
    }
}
