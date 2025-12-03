using System;

namespace SWP391_Project.Models;

public class Location
{
    public int Id { get; set; }

    /// <summary>
    /// Thành phố (dùng cho filter Location).
    /// </summary>
    public required string City { get; set; }

    /// <summary>
    /// Quận/huyện/phường.
    /// </summary>
    public required string Ward { get; set; }

    /// <summary>
    /// Các Company thuộc Location này.
    /// </summary>
    public List<Company> Companies { get; set; } = new();

    /// <summary>
    /// Các Job thuộc Location này.
    /// </summary>
    public List<Job> Jobs { get; set; } = new();
}
