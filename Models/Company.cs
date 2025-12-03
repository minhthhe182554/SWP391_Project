using System;

namespace SWP391_Project.Models;

public class Company
{
    /// <summary>
    /// Khóa chính của Company.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// FK tới User tương ứng (tài khoản recruiter/company).
    /// Dùng cho đăng nhập và phân quyền.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Navigation 1-1 tới User.
    /// </summary>
    public User User { get; set; } = null!;

    public required string Name { get; set; }

    public required string PhoneNumber { get; set; }

    public required string Description { get; set; }

    /// <summary>
    /// FK tới Location của công ty (dùng cho filter theo Location).
    /// </summary>
    public int LocationId { get; set; }

    /// <summary>
    /// Địa chỉ chi tiết (số nhà, đường, ...).
    /// </summary>
    public required string Address { get; set; }

    /// <summary>
    /// Navigation tới Location.
    /// </summary>
    public Location Location { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public string? Website { get; set; }

    /// <summary>
    /// Danh sách Job mà Company đã đăng (1-n).
    /// Dùng cho Recruiter Dashboard và Company detail.
    /// </summary>
    public List<Job> Jobs { get; set; } = new();
}
