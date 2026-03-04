using KolsheBjam3etna.DAL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Request
{
    public class CreateSwapAdRequest
    {
        public string OfferTitle { get; set; } = "";

        public string WantedTitle { get; set; } = "";

        public int CategoryId { get; set; }

        public ProductCondition Condition { get; set; }

        public string Description { get; set; } = "";

        public List<IFormFile>? Images { get; set; }
    }
}
