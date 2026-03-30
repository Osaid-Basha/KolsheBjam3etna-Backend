using System;

namespace KolsheBjam3etna.DAL.DTOs.Response
{
    public class ClubDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string UniversityName { get; set; } = null!;

        public string ManagerName { get; set; } = null!;
        public string ManagerEmail { get; set; } = null!;

        public string SubscriptionType { get; set; } = null!;
        public decimal SubscriptionPrice { get; set; }

        public DateTime SubscriptionStartDate { get; set; }
        public DateTime SubscriptionEndDate { get; set; }

        public string SubscriptionStatus { get; set; } = null!;
    }
}