using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
namespace KolsheBjam3etna.DAL.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string FullName { get; set; }
        public int? UniversityId { get; set; }

        public University University { get; set; }

        public string? Major { get; set; }

        public string? Bio { get; set; }

        public string? ProfileImageUrl { get; set; }

        public bool IsProfileCompleted { get; set; } = false;
    }
}
