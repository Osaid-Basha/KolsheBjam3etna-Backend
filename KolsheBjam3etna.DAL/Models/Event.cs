using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Models
{
    public class Event
    {
        public int Id { get; set; }

        public string CoordinatorId { get; set; } = default!;
        public ApplicationUser Coordinator { get; set; } = default!;

        public string Title { get; set; } = "";
        public string Type { get; set; } = "";
        public string Location { get; set; } = "";
        public DateTime DateTimeUtc { get; set; }
        public int Capacity { get; set; }

        public string Description { get; set; } = ""; // short
        public string? Content { get; set; }          // ✅ long/rich content

        public string? CoverImageUrl { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public List<EventAgendaItem> Agenda { get; set; } = new(); // ✅
        public List<EventRegistration> Registrations { get; set; } = new();
    }
}
