using System.Text.Json.Serialization;

namespace KolsheBjam3etna.DAL.DTOs.Request
{
    public class CreateDmByEmailRequest
    {
        [JsonPropertyName("otherUserEmail")]
        public string OtherUserEmail { get; set; } = "";
    }
}
