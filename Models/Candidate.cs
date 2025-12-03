using System;

namespace SWP391_Project.Models;

public class Candidate
{
    /// <summary>
    /// Khóa chính của Candidate.
    /// Thêm Id riêng để EF Core có thể map entity độc lập với User.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// FK tới User tương ứng (tài khoản đăng nhập).
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Navigation 1-1 tới User.
    /// Cho phép include từ Candidate sang User và ngược lại.
    /// </summary>
    public User User { get; set; } = null!;

    public required string FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? ImageUrl { get; set; }

    /// <summary>
    /// Học vấn của Candidate (1-n).
    /// </summary>
    public List<EducationRecord> EducationRecords { get; set; } = new();

    /// <summary>
    /// Kinh nghiệm làm việc của Candidate (1-n).
    /// </summary>
    public List<WorkExperience> WorkExperiences { get; set; } = new();

    /// <summary>
    /// Danh sách chứng chỉ, được map qua entity Certificate (1-n).
    /// </summary>
    public List<Certificate> Certificates { get; set; } = new();

    /// <summary>
    /// Kỹ năng của Candidate (many-to-many với Skill).
    /// EF Core sẽ tự tạo bảng join CandidateSkill.
    /// </summary>
    public List<Skill> Skills { get; set; } = new();

    /// <summary>
    /// Các CV đã upload (1-n).
    /// Dùng cho bước 1 của luồng Apply Job: chọn CV để apply.
    /// </summary>
    public List<Resume> Resumes { get; set; } = new();

    /// <summary>
    /// Các application mà Candidate đã gửi (1-n).
    /// Thay cho việc lưu trực tiếp List&lt;Job&gt; AppliedJobs để có thêm thông tin CoverLetter, ResumeUrl,...
    /// </summary>
    public List<Application> Applications { get; set; } = new();

    /// <summary>
    /// Các job đã được Candidate lưu (Saved Jobs) thông qua bảng trung gian SavedJob.
    /// </summary>
    public List<SavedJob> SavedJobs { get; set; } = new();

    /// <summary>
    /// Notification gửi cho Candidate (1-n).
    /// </summary>
    public List<Notification> Notifications { get; set; } = new();

    /// <summary>
    /// Các report do Candidate gửi (Job Report).
    /// </summary>
    public List<Report> Reports { get; set; } = new();

    /// <summary>
    /// Flag cho biết Candidate đang thất nghiệp hay không.
    /// Có thể hỗ trợ logic gợi ý job, thống kê.
    /// </summary>
    public bool Jobless { get; set; } = true;

    /// <summary>
    /// Số lượng report còn lại trong tháng (đáp ứng "Report limited per account per month").
    /// </summary>
    public int RemainingReport { get; set; } = 2;
}
