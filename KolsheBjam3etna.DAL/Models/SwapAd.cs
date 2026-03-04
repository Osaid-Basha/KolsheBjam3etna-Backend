using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Models
{
    public class SwapAd
    {
        public int Id { get; set; }

        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;

        public string OfferTitle { get; set; } = "";   

        public string WantedTitle { get; set; } = ""; 

        public int CategoryId { get; set; }

        public ProductCategory Category { get; set; } = default!;

        public ProductCondition Condition { get; set; }

        public string Description { get; set; } = "";

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public List<SwapAdImage> Images { get; set; } = new();
    }
}
