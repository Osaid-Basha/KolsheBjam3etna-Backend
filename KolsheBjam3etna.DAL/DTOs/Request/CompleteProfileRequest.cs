using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace KolsheBjam3etna.DAL.DTOs.Request
{
    public class CompleteProfileRequest
    {
        public int UniversityId { get; set; }

        public string Major { get; set; }

        public string Bio { get; set; }

        public IFormFile? ProfileImageUrl { get; set; }
    }
}
