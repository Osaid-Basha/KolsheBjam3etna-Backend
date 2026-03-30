using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Request
{
    public class UpdateClubRequest
    {
        public string Name { get; set; } = null!;
        public string UniversityName { get; set; } = null!;
        public string ManagerName { get; set; } = null!;
        public string ManagerEmail { get; set; } = null!;
        public string SubscriptionType { get; set; } = null!;
    }
}