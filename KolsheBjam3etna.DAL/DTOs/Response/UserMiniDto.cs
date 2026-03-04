using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Response
{
    public class UserMiniDto
    {
        public string Id { get; set; } = "";
        public string FullName { get; set; } = "";
        public string? ProfileImageUrl { get; set; }
        public string? Major { get; set; }
        public int? StudyYear { get; set; }
        public int? UniversityId { get; set; }
    }

}
