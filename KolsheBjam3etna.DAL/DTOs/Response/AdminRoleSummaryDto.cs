using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Response
{
    public class AdminRoleSummaryDto
    {
        public string Role { get; set; } = "";
        public int UsersCount { get; set; }
    }
}
