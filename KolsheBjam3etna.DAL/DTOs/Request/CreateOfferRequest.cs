using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Request
{
    public class CreateOfferRequest
    {
        public string TargetType { get; set; } = ""; 
        public int TargetId { get; set; }

        public decimal Price { get; set; }
        public string Availability { get; set; } = "";
        public string? Message { get; set; }
    }
}
