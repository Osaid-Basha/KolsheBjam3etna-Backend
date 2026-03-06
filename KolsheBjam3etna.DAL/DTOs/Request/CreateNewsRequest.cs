using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Request
{
    public class CreateNewsRequest
    {
        public IFormFile? Image { get; set; }

        public string Title { get; set; }
        public string Source { get; set; }
        public string Category { get; set; }
        public string Content { get; set; }

        public bool IsImportant { get; set; }
        public bool IsPublished { get; set; }
    }
}
