using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Request
{
    public class SendMessageRequest
    {
        public int ConversationId { get; set; }
        public string Text { get; set; } = "";
    }
}
