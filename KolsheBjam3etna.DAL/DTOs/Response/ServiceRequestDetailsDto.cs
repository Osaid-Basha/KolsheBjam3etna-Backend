using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Response
{
  

        public class ServiceRequestDetailsDto
        {
            public int Id { get; set; }
            public string Title { get; set; } = "";
            public decimal Budget { get; set; }
            public DateTime DeadlineUtc { get; set; }
            public string Description { get; set; } = "";
            public int CategoryId { get; set; }
            public string CategoryName { get; set; } = "";
            public string UserId { get; set; } = "";
            public DateTime CreatedAtUtc { get; set; }

            public List<ServiceRequestAttachmentDto> Attachments { get; set; } = new();
              public UserMiniDto User { get; set; } = new();
    }

        public class ServiceRequestAttachmentDto
        {
            public int Id { get; set; }
            public string FileUrl { get; set; } = "";
            public string FileName { get; set; } = "";
            public long Size { get; set; }
            public DateTime UploadedAtUtc { get; set; }
        }
    
}
