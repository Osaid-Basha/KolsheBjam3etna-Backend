using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Request
{
    public class UpdateAcademicProfileRequest
    {
        public int UniversityId { get; set; }
        public string Major { get; set; }
        public int StudyYear { get; set; }
        public string UniversityNumber { get; set; }
    }
}
