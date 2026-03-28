using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Response
{
    public class ProfileResponse
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Bio { get; set; }

        public string? WebsiteUrl { get; set; }

        public string? ProfileImageUrl { get; set; }
        public string? UniversityName { get; set; }
        public int? UniversityId { get; set; }

        public string? Major { get; set; }

        public int? StudyYear { get; set; }

        public string? UniversityNumber { get; set; }
    }
}
