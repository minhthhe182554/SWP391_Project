using System;

namespace SWP391_Project.Models;

public class Job
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public required string Description { get; set; }

    /// <summary>
    /// Thời gian làm việc (full-time/part-time/shift, mô tả chi tiết).
    /// </summary>
    public required string WorkTime { get; set; }

    public JobType Type { get; set; } = JobType.FULLTIME;

    /// <summary>
    /// Số năm kinh nghiệm yêu cầu.
    /// </summary>
    public required int YearsOfExperience { get; set; }

    /// <summary>
    /// Số lượng vị trí cần tuyển.
    /// </summary>
    public required int VacancyCount { get; set; }

    /// <summary>
    /// Khoảng lương: min - max, dùng cho filter Salary Range.
    /// </summary>
    public decimal? LowerSalaryRange { get; set; }

    public decimal? HigherSalaryRange { get; set; }

    public required DateTime StartDate { get; set; }

    public required DateTime EndDate { get; set; }

    /// <summary>
    /// Cờ soft delete: khi Company "xoá" job, chỉ set IsDelete = true.
    /// Tất cả truy vấn job active nên filter IsDelete == false.
    /// </summary>
    public bool IsDelete { get; set; } = false;

    /// <summary>
    /// FK tới Company đăng job.
    /// Phục vụ Recruiter Dashboard và Company detail.
    /// </summary>
    public int CompanyId { get; set; }

    public Company Company { get; set; } = null!;

    /// <summary>
    /// FK tới Location nơi làm việc.
    /// Hỗ trợ filter theo Location (City/Ward).
    /// </summary>
    public int LocationId { get; set; }

    public Location Location { get; set; } = null!;

    /// <summary>
    /// Các kỹ năng yêu cầu cho job (many-to-many với Skill).
    /// </summary>
    public List<Skill> RequiredSkills { get; set; } = new();

    /// <summary>
    /// Các domain/ngành nghề liên quan (many-to-many với Domain).
    /// </summary>
    public List<Domain> Domains { get; set; } = new();

    /// <summary>
    /// Các application apply vào job này (1-n).
    /// Dùng cho màn hình Job Applicants List & export.
    /// </summary>
    public List<Application> Applications { get; set; } = new();

    /// <summary>
    /// Các report liên quan tới job này (Job Report).
    /// </summary>
    public List<Report> Reports { get; set; } = new();
}
