using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Response
{
    namespace KolsheBjam3etna.DAL.DTOs.Response
    {
        public class MessageImageResponse
        {
            public long Id { get; set; }
            public string ImageUrl { get; set; } = default!;
            public int SortOrder { get; set; }
        }

        public class MessageResponse
        {
            public long Id { get; set; }
            public int ConversationId { get; set; }
            public string SenderId { get; set; } = default!;

            public string? Text { get; set; }

            public List<MessageImageResponse> Images { get; set; } = new();

            public string? FileUrl { get; set; }
            public string? FileName { get; set; }
            public long? FileSize { get; set; }
            public string? FileContentType { get; set; }

            public DateTime SentAtUtc { get; set; }
            public bool IsRead { get; set; }

            public bool IsEdited { get; set; }
            public DateTime? EditedAtUtc { get; set; }

            public bool IsDeleted { get; set; }
            public DateTime? DeletedAtUtc { get; set; }
        }
    }
}
