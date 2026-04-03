using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.AspNetCore.Http;

namespace KolsheBjam3etna.DAL.DTOs.Request
{
    public class SendMessageRequest
    {
        public int ConversationId { get; set; }
        public string? Text { get; set; }
        public List<IFormFile>? Images { get; set; }
        public IFormFile? File { get; set; }
    }
}
