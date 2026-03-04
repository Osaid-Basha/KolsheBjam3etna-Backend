using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Request
{
    public class VerifyResetCodeRequest
    {
        public string Email { get; set; } = "";
        public string Code { get; set; } = "";
    }
}
