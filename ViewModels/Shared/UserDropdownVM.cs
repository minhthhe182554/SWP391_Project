namespace SWP391_Project.ViewModels.Shared
{
    public class UserDropdownVM
    {
        public string UserId { get; set; } = "N/A";
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = "User";
        public string Role { get; set; } = "CANDIDATE";
        public string AvatarUrl { get; set; } = "/imgs/ic_default_avatar.png";
    }
}


