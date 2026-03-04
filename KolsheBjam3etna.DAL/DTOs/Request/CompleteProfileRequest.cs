using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Request
{
    public class CompleteProfileRequest
    {
        public int UniversityId { get; set; }

        public string Major { get; set; }

        public string Bio { get; set; }

        public string? ProfileImageUrl { get; set; }
    }
}
