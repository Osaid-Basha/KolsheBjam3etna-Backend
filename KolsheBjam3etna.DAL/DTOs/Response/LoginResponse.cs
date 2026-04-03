using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Response
{
    public class LoginResponse
    {
        public string Message { get; set; }
        public string? Token { get; set; }
        public bool IsProfileCompleted { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
