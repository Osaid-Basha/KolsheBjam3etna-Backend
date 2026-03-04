using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace KolsheBjam3etna.DAL.DTOs.Request
{
    public class CreateDmRequest
    {
        [JsonPropertyName("otherUserId")]
        public string OtherUserId { get; set; } = default!;
    }
}
