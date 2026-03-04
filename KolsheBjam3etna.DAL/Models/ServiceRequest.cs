using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Models
{
   
        public class ServiceRequest
        {
            public int Id { get; set; }

            public string UserId { get; set; } = default!;
            public ApplicationUser User { get; set; } = default!;

            public string Title { get; set; } = "";
            public int CategoryId { get; set; }
            public ServiceCategory Category { get; set; } = default!;

            public decimal Budget { get; set; }
            public DateTime DeadlineUtc { get; set; }

            public string Description { get; set; } = "";

            public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

            public List<ServiceRequestAttachment> Attachments { get; set; } = new();
        }
    
}
