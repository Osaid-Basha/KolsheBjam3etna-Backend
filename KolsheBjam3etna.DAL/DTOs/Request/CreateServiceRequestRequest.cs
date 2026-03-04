using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Request
{
    public class CreateServiceRequestRequest
    {
        [Required] public string Title { get; set; } = "";
        [Required] public int CategoryId { get; set; }

        [Required] public decimal Budget { get; set; }
        [Required] public DateTime DeadlineUtc { get; set; }

        [Required] public string Description { get; set; } = "";

       
        public List<IFormFile>? Files { get; set; }
    }

}
