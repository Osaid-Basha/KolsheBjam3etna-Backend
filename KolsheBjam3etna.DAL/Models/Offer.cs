using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Models
{
    public enum OfferStatus
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2
    }

    public enum OfferTargetType
    {
        ServiceRequest = 0,
        ProductAd = 1,
        SwapAd = 2
    }

    public class Offer
    {
        public int Id { get; set; }

        public string SenderId { get; set; } = default!;
        public ApplicationUser Sender { get; set; } = default!;

      
        public string ReceiverId { get; set; } = default!;
        public ApplicationUser Receiver { get; set; } = default!;

        public OfferTargetType TargetType { get; set; }
        public int TargetId { get; set; }

        public decimal Price { get; set; }
        public string Availability { get; set; } = ""; 
        public string? Message { get; set; }          

        public OfferStatus Status { get; set; } = OfferStatus.Pending;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? RespondedAtUtc { get; set; }
    }
}
