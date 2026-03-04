using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Models
{
    public class ServiceRequestAttachment
    {
        public int Id { get; set; }

        public int ServiceRequestId { get; set; }
        public ServiceRequest ServiceRequest { get; set; } = default!;

        public string FileUrl { get; set; } = "";   
        public string FileName { get; set; } = "";
        public long Size { get; set; }
        public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
