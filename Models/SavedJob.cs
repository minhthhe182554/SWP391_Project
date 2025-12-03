namespace SWP391_Project.Models;

/// <summary>
/// Bảng trung gian many-to-many giữa Candidate và Job cho chức năng "Saved Jobs".
/// Lưu thêm thời điểm lưu để có thể thống kê hoặc sort.
/// </summary>
public class SavedJob
{
    public int CandidateId { get; set; }

    public Candidate Candidate { get; set; } = null!;

    public int JobId { get; set; }

    public Job Job { get; set; } = null!;
}


