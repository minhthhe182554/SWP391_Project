using System.ComponentModel.DataAnnotations;

namespace SWP391_Project.ViewModels
{
    public class ForgotPasswordVM
    {
        [Required(ErrorMessage = "Vui Long nhap Email")]
        [EmailAddress(ErrorMessage = "Email khong dung dinh dang")]
        public string Email { get; set; }
    }
}
