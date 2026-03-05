using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Request
{
    

    public class CreateEventRequest
    {
        public IFormFile? CoverImage { get; set; }

        public string Title { get; set; } = "";
        public string Type { get; set; } = "";
        public string Location { get; set; } = "";

        public DateTime DateTimeUtc { get; set; }
        public int Capacity { get; set; }

        public string Description { get; set; } = "";
        public string? Content { get; set; }

    
        public string? AgendaJson { get; set; }
    }
}
