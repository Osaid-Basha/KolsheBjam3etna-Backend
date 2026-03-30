using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Response
{
    public class RoleOptionDto
    {
        public string Role { get; set; } = null!;
        public List<string> Permissions { get; set; } = new();
    }
}
