using System;

namespace SWP391_Project.Models;

public class User
{
    public int Id { get; set; }

    public required string Email { get; set; }

    public required string Password { get; set; }

    /// <summary>
    /// Trạng thái kích hoạt của tài khoản.
    /// Dùng cho chức năng ban/unban user (Admin).
    /// </summary>
    public bool Active { get; set; } = true;

    /// <summary>
    /// Vai trò: Candidate / Company / Admin.
    /// Dùng để redirect đến Dashboard/Trang phù hợp sau khi Login.
    /// </summary>
    public required Role Role { get; set; }

    /// <summary>
    /// Quan hệ 1-1 với Candidate (nếu Role = Candidate).
    /// Cho phép truy ngược từ User sang profile ứng viên.
    /// </summary>
    public Candidate? Candidate { get; set; }

    /// <summary>
    /// Quan hệ 1-1 với Company (nếu Role = Company).
    /// Cho phép truy ngược từ User sang profile công ty.
    /// </summary>
    public Company? Company { get; set; }
}
