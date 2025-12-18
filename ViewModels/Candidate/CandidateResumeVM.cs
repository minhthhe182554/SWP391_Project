namespace SWP391_Project.ViewModels.Candidate;

public class CandidateResumeVM
{
    public int CandidateId { get; set; }
    public List<CandidateResumeItemVM> Resumes { get; set; } = new();
    public bool HasResumes => Resumes.Any();
}

public class CandidateResumeItemVM
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? OriginalFileName { get; set; }
    public List<string> PreviewUrls { get; set; } = new();
}
