using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Response
{
    public class AdminRoleUserDto
    {
        public string UserId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;

        public int? ClubId { get; set; }
        public string? ClubName { get; set; }

        public DateTime CreatedAt { get; set; }
        public List<string> Permissions { get; set; } = new();
    }
}
