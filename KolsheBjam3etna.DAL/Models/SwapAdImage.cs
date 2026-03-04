using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Models
{
    public class SwapAdImage
    {
        public int Id { get; set; }

        public int SwapAdId { get; set; }

        public SwapAd SwapAd { get; set; } = default!;

        public string ImageUrl { get; set; } = "";

        public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
