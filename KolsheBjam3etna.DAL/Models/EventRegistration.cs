using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Models
{
    public class EventRegistration
    {
        public int Id { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; } = default!;

        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;
        public string FullName { get; set; } = "";
        public string UniversityEmail { get; set; } = "";
        public int StudyYear { get; set; }  // 1..4
        public DateTime RegisteredAtUtc { get; set; } = DateTime.UtcNow;
    }
}
