using System.Collections.Generic;

namespace SWP391_Project.ViewModels.Company
{
    public class CompanyDetailVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Website { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? Ward { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string ImageUrl { get; set; } = "/imgs/ic_default_avatar.png";
        public List<string> Domains { get; set; } = new();
        public List<CompanyJobItemVM> Jobs { get; set; } = new();
    }
}

