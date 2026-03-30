using System.Collections.Generic;

namespace KolsheBjam3etna.DAL.Models
{
    public class Club
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string UniversityName { get; set; } = string.Empty;

        public string ManagerName { get; set; } = string.Empty;
        public string ManagerEmail { get; set; } = string.Empty;

        public string SubscriptionType { get; set; } = string.Empty;
        public decimal SubscriptionPrice { get; set; }

        public DateTime SubscriptionStartDate { get; set; }
        public DateTime SubscriptionEndDate { get; set; }

        public string? OwnerId { get; set; }
        public ApplicationUser? Owner { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public List<Event> Events { get; set; } = new();
    }
}