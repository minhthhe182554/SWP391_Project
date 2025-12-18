namespace SWP391_Project.Models;

public class Candidate
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public required string FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ImageUrl { get; set; } = "default_yvl9oh";
    public List<EducationRecord> EducationRecords { get; set; } = new();
    public List<WorkExperience> WorkExperiences { get; set; } = new();
    public List<Certificate> Certificates { get; set; } = new();
    public List<Skill> Skills { get; set; } = new();
    public List<Resume> Resumes { get; set; } = new();
    public List<Application> Applications { get; set; } = new();
    public List<SavedJob> SavedJobs { get; set; } = new();
    public List<Notification> Notifications { get; set; } = new();
    public List<Report> Reports { get; set; } = new();
    public bool Jobless { get; set; } = true;
    public int RemainingReport { get; set; } = 2;
}
