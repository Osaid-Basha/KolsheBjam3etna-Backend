using KolsheBjam3etna.DAL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Request
{
    public class CreateProductAdRequest
    {
        [Required]
        public string Title { get; set; } = "";

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public ProductCondition Condition { get; set; }

        [Required]
        public string Description { get; set; } = "";

        public List<IFormFile>? Images { get; set; }
    }
}
