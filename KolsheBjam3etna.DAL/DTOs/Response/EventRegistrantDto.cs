using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Response
{
    public class EventRegistrantDto
    {
        public string UserId { get; set; } = "";
        public string FullName { get; set; } = "";
        public string? ProfileImageUrl { get; set; }
        public string? Major { get; set; }
        public int? StudyYear { get; set; }
        public int? UniversityId { get; set; }
        public DateTime RegisteredAtUtc { get; set; }
    }
}
