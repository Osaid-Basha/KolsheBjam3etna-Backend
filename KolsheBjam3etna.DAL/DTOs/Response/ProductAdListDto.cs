using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Response
{
    public class ProductAdListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public decimal Price { get; set; }
        public string Condition { get; set; } = "";

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";

        public string UserId { get; set; } = "";
        public UserMiniDto User { get; set; } = new();

        public string? CoverImageUrl { get; set; } // أول صورة
        public DateTime CreatedAtUtc { get; set; }
    }
}
