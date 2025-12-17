using System.Collections.Generic;
using SWP391_Project.Models;

namespace SWP391_Project.ViewModels.Admin;

public class ManageReportsVM
{
    public List<ReportItemVM> Reports { get; set; } = new();
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public int Total { get; set; }
    public int PageSize { get; set; } = 10;

    public int? FocusReportId { get; set; }
}

public class ReportItemVM
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string CandidateName { get; set; } = string.Empty;
    public int CandidateUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public ReportStatus Status { get; set; }
}

