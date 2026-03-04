using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Request
{
    public class AdminUserDetailsDto
    {
        public string Id { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? ProfileImageUrl { get; set; }

        public string? Major { get; set; }
        public int? StudyYear { get; set; }
        public string? UniversityNumber { get; set; }
        public string? Bio { get; set; }
        public string? WebsiteUrl { get; set; }

        public string? UniversityName { get; set; }

        public bool IsBlocked { get; set; }
        public List<string> Roles { get; set; } = new();

        public int ProductAdsCount { get; set; }
        public int SwapAdsCount { get; set; }
        public int ServiceRequestsCount { get; set; }
    }
}
