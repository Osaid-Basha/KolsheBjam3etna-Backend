using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Request
{
    public class RenewClubSubscriptionRequest
    {
        public string SubscriptionType { get; set; } = null!; // Monthly / Yearly
    }
}
