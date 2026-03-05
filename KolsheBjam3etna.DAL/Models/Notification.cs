using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Models
{
    public enum NotificationType
    {
        Offer = 0,
        Message = 1,
        EventReminder = 2,
        Deadline = 3,
        Announcement = 4
    }

    public class Notification
    {
        public int Id { get; set; }

        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;

        public NotificationType Type { get; set; }

        public string Title { get; set; } = "";
        public string Body { get; set; } = "";

        // للتوجيه بالفرونت (افتح صفحة offer/detail/event…)
        public string? TargetType { get; set; } // "Offer" "ServiceRequest" "Event" "Chat" ...
        public int? TargetId { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
