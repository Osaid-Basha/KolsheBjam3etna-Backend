using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Response
{
    public class ServiceRequestListItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public decimal Budget { get; set; }
        public DateTime DeadlineUtc { get; set; }
        public string Description { get; set; } = "";

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";

        public string UserId { get; set; } = "";
        public UserMiniDto User { get; set; } = new();

        public DateTime CreatedAtUtc { get; set; }

        public int AttachmentsCount { get; set; }
    }
}
