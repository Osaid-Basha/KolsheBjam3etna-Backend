using Microsoft.AspNetCore.Identity;

namespace KolsheBjam3etna.DAL.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        public int? UniversityId { get; set; }
        public University? University { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? Major { get; set; }

        public int? StudyYear { get; set; }

        public string? UniversityNumber { get; set; }

        public string? Bio { get; set; }

        public string? WebsiteUrl { get; set; }

        public string? ProfileImageUrl { get; set; }

        public bool IsProfileCompleted { get; set; } = false;

        public int? ManagedEventId { get; set; }
        public Event? ManagedEvent { get; set; }
    }
}