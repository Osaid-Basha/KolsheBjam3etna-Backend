using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Response
{
    public class EventBasicDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int? ClubId { get; set; }
        public string? ClubOwnerId { get; set; }
    }
}
