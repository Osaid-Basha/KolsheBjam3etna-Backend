using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Response
{
    public class OfferCardDto
    {
        public int Id { get; set; }

        public string TargetType { get; set; } = "";
        public int TargetId { get; set; }
        public string TargetTitle { get; set; } = ""; // عنوان المنشور

        public decimal Price { get; set; }
        public string Availability { get; set; } = "";
        public string? Message { get; set; }

        public string Status { get; set; } = ""; // Pending/Accepted/Rejected
        public DateTime CreatedAtUtc { get; set; }

        // الطرف الآخر
        public string OtherUserId { get; set; } = "";
        public string OtherUserName { get; set; } = "";
        public string? OtherUserProfileImageUrl { get; set; }
        public double? OtherUserRating { get; set; } // إذا عندك تقييمات لاحقاً
    }
}
