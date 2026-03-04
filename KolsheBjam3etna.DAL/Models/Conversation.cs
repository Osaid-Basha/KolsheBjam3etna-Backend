using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Models
{
    public class Conversation
    {
        public int Id { get; set; }

        public string User1Id { get; set; } = default!;
        public ApplicationUser User1 { get; set; } = default!;

        public string User2Id { get; set; } = default!;
        public ApplicationUser User2 { get; set; } = default!;

        public string? LastMessageText { get; set; }

        public DateTime? LastMessageAtUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
