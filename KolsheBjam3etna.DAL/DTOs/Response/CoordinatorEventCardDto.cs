using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Response
{
    namespace KolsheBjam3etna.DAL.DTOs.Response
    {
        public class CoordinatorEventCardDto
        {
            public int Id { get; set; }
            public string Title { get; set; } = "";
            public string Type { get; set; } = "";
            public string Location { get; set; } = "";
            public DateTime DateTimeUtc { get; set; }

            public int Capacity { get; set; }
            public int RegisteredCount { get; set; }
            public int ProgressPercent { get; set; } // Registered/Capacity * 100

            public string? CoverImageUrl { get; set; }
        }
    }
}
