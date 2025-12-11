using SWP391_Project.Models;

namespace SWP391_Project.ViewModels.Candidate;

public class CandidateProfileVM
{
    public int CandidateId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool Jobless { get; set; }
    public int RemainingReport { get; set; }
    public List<EducationRecord> EducationRecords { get; set; } = new();
    public List<WorkExperience> WorkExperiences { get; set; } = new();
    public List<Certificate> Certificates { get; set; } = new();
    public List<Skill> Skills { get; set; } = new();
}
