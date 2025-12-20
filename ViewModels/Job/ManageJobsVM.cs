using SWP391_Project.Models.Enums;

namespace SWP391_Project.ViewModels.Job
{
    public class ManageJobsVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string CityName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ApplicationCount { get; set; }
        public JobType JobType { get; set; }

        public bool IsDelete { get; set; }
        public bool IsExpired  => DateTime.Now > EndDate;
        public string StatusLabel => IsExpired
            ? "<span class='badge bg-danger'>Hết hạn</span>"
            : "<span class='badge bg-success'>Đang tuyển</span>";
    }
}
