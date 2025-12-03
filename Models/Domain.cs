using System;

namespace SWP391_Project.Models;

public class Domain
{
    public int Id { get; set; }

    public required string Name { get; set; }

    /// <summary>
    /// Các Job thuộc domain/ngành nghề này (many-to-many).
    /// </summary>
    public List<Job> Jobs { get; set; } = new();
}
