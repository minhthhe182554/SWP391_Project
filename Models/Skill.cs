using System;

namespace SWP391_Project.Models;

public class Skill
{
    public int Id { get; set; }

    public required string Name { get; set; }

    /// <summary>
    /// Các Candidate sở hữu kỹ năng này (many-to-many).
    /// </summary>
    public List<Candidate> Candidates { get; set; } = new();

    /// <summary>
    /// Các Job yêu cầu kỹ năng này (many-to-many).
    /// </summary>
    public List<Job> Jobs { get; set; } = new();
}
