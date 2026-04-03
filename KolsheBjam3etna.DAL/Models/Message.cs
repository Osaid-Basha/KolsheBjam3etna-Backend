using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Models
{
    public enum MessageType
    {
        Text = 1,
        Image = 2,
        File = 3,
        Mixed = 4
    }

    public class Message
    {
        public long Id { get; set; }

        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; } = default!;

        public string SenderId { get; set; } = default!;
        public ApplicationUser Sender { get; set; } = default!;

        public MessageType Type { get; set; } = MessageType.Text;

        public string? Text { get; set; }

        public string? FileUrl { get; set; }
        public string? FileName { get; set; }
        public long? FileSize { get; set; }
        public string? FileContentType { get; set; }

        public DateTime SentAtUtc { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        public bool IsEdited { get; set; } = false;
        public DateTime? EditedAtUtc { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAtUtc { get; set; }

        public ICollection<MessageImage> Images { get; set; } = new List<MessageImage>();
    }
}