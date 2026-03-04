using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Response
{
    public class ChatListItemResponse
    {
        public int ConversationId { get; set; }

        public string OtherUserId { get; set; } = "";
        public string OtherFullName { get; set; } = "";
        public string? OtherProfileImageUrl { get; set; }

        public string? LastMessageText { get; set; }
        public DateTime? LastMessageAtUtc { get; set; }

        public int UnreadCount { get; set; }
    }

}
