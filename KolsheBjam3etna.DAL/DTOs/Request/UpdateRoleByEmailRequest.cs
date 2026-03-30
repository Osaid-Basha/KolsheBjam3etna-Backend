using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Request
{
    public class UpdateRoleByEmailRequest
    {
        public string Email { get; set; } = null!;
        public string NewRole { get; set; } = null!;
        public int? ClubId { get; set; }
    }
}
