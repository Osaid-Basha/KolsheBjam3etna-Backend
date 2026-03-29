using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace KolsheBjam3etna.DAL.DTOs.Request
{
    public class UpdateNewsRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Content { get; set; }

        [Required]
        [MaxLength(150)]
        public string Source { get; set; }

        [Required]
        [MaxLength(100)]
        public string Category { get; set; }

        public IFormFile? Image { get; set; }

        public bool IsImportant { get; set; }

        public bool IsPublished { get; set; }
    }
}