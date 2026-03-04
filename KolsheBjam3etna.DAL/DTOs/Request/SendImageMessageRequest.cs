using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Request
{
    public class SendImageMessageRequest
    {
        public int ConversationId { get; set; }
        public IFormFile Image { get; set; } = default!;
        public string? Caption { get; set; } 
    }
}
