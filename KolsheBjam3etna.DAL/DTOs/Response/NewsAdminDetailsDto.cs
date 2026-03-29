using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Response
{
    public class NewsAdminDetailsDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string Source { get; set; }

        public string Category { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsImportant { get; set; }

        public bool IsPublished { get; set; }

        public int ViewsCount { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
