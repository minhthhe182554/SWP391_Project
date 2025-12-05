using System;

namespace SWP391_Project.Models;

public class Notification
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public bool Read { get; set; } = false;
    public NotificationType Type { get; set; } = NotificationType.OTHERS;
    public int? NavigationId { get; set; }
    public int CandidateId { get; set; }
    public Candidate Candidate { get; set; } = null!;
}
