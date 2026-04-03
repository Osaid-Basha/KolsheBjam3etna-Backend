using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Models
{
    public class MessageImage
    {
        public long Id { get; set; }

        public long MessageId { get; set; }
        public Message Message { get; set; } = default!;

        public string ImageUrl { get; set; } = default!;
        public int SortOrder { get; set; }
    }
}
