using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Request
{

    public class AssignRoleByEmailRequest
    {
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";  
    }
}
