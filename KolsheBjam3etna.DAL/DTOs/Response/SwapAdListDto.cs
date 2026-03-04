using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Response
{
    public class SwapAdListDto
    {
        public int Id { get; set; }
        public string OfferTitle { get; set; } = "";
        public string WantedTitle { get; set; } = "";
        public string Condition { get; set; } = "";
        public string Description { get; set; } = "";

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";

        public string UserId { get; set; } = "";
        public UserMiniDto User { get; set; } = new();

        public string? CoverImageUrl { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
