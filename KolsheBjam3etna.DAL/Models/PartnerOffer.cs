using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Models
{
    public enum PartnerOfferType
    {
        Academy = 0,
        Store = 1
    }

    public class PartnerOffer
    {
        public int Id { get; set; }

        public string PartnerName { get; set; } = "";
        public PartnerOfferType Type { get; set; }

        public string Category { get; set; } = "";      
        public string Title { get; set; } = "";         
        public string Description { get; set; } = "";

        public string Location { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";

        public string? ImageUrl { get; set; }

        public int DiscountPercent { get; set; }

        public bool IsVerified { get; set; } = true;
        public bool ShowOnHomePage { get; set; } = true;

        public double Rating { get; set; } = 0;
        public int RatingsCount { get; set; } = 0;

        public DateTime ExpireDateUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
