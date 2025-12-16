using System.Collections.Generic;
using SWP391_Project.Models;

namespace SWP391_Project.ViewModels.Admin;

public class ManageUsersVM
{
    public List<UserItemVM> ActiveCandidates { get; set; } = new();
    public List<UserItemVM> BannedCandidates { get; set; } = new();
    public int CandidateActivePage { get; set; } = 1;
    public int CandidateActiveTotalPages { get; set; } = 1;
    public int CandidateActiveTotal { get; set; }
    public int CandidateBannedPage { get; set; } = 1;
    public int CandidateBannedTotalPages { get; set; } = 1;
    public int CandidateBannedTotal { get; set; }

    public List<UserItemVM> ActiveCompanies { get; set; } = new();
    public List<UserItemVM> BannedCompanies { get; set; } = new();
    public int CompanyActivePage { get; set; } = 1;
    public int CompanyActiveTotalPages { get; set; } = 1;
    public int CompanyActiveTotal { get; set; }
    public int CompanyBannedPage { get; set; } = 1;
    public int CompanyBannedTotalPages { get; set; } = 1;
    public int CompanyBannedTotal { get; set; }

    public int PageSize { get; set; } = 10;

    // Metrics for quick charts/cards
    public int TotalCandidatesCount { get; set; }
    public int ActiveCandidatesCount { get; set; }
    public int BannedCandidatesCount { get; set; }

    public int TotalCompaniesCount { get; set; }
    public int ActiveCompaniesCount { get; set; }
    public int BannedCompaniesCount { get; set; }

    // Job metrics
    public int TotalJobs { get; set; }
    public int ActiveJobs { get; set; }
}

public class UserItemVM
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public Role Role { get; set; }
    public bool Active { get; set; }
    public string Status => Active ? "Active" : "Banned";
}

