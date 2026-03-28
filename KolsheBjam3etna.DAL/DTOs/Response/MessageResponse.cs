using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Response
{
    public class MessageResponse
    {
        public long Id { get; set; }
        public int ConversationId { get; set; }
        public string SenderId { get; set; } = "";
        public string? Text { get; set; }
        public string? ImageUrl { get; set; } 
        public DateTime SentAtUtc { get; set; }
        public bool IsRead { get; set; }
    }
}
