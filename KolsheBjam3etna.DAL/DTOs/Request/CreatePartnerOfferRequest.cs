using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Request
{
    public class CreatePartnerOfferRequest
    {
        public IFormFile? Image { get; set; }

        public string PartnerName { get; set; } = "";
        public string Type { get; set; } = "";

        public string Category { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";

        public string Location { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";

        public DateTime ExpireDateUtc { get; set; }
        public int DiscountPercent { get; set; }

        public bool ShowOnHomePage { get; set; } = true;
    }
}
