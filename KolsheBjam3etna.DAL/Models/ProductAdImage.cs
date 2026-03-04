using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Models
{
    public class ProductAdImage
    {
        public int Id { get; set; }

        public int ProductAdId { get; set; }
        public ProductAd ProductAd { get; set; } = default!;

        public string ImageUrl { get; set; } = "";

        public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
