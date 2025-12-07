using System.ComponentModel.DataAnnotations;

namespace SWP391_Project.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Vui long nhap email!")]
        [EmailAddress(ErrorMessage = "Email khong dung dinh dang")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Vui long nhap mat khau")]
        public string Password { get; set; } = null!;
    }
}
