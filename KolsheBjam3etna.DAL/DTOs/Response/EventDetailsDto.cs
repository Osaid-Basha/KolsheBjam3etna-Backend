using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Response
{
    public class EventDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Type { get; set; } = "";
        public string Location { get; set; } = "";
        public DateTime DateTimeUtc { get; set; }
        public int Capacity { get; set; }
        public int RegisteredCount { get; set; }
        public string Description { get; set; } = "";
        public string? CoverImageUrl { get; set; }

        // معلومات بسيطة عن المنسق (إذا بدك)
        public string CoordinatorId { get; set; } = "";
        public string CoordinatorName { get; set; } = "";
        public string? CoordinatorProfileImageUrl { get; set; }
        public string? Content { get; set; }
        public List<EventAgendaItemDto> Agenda { get; set; } = new();
    }
}
