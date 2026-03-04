using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Request
{
    public class RegisterEventRequest
    {
        public string FullName { get; set; } = "";
        public string UniversityEmail { get; set; } = "";
        public int StudyYear { get; set; } // 1..4
    }
}
