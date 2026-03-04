using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Response
{
    public class AdminUserListItemDto
    {
        public string Id { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? ProfileImageUrl { get; set; }

        public string? UniversityName { get; set; }

        public int PostsCount { get; set; }      
        public bool IsBlocked { get; set; }
    }
}
