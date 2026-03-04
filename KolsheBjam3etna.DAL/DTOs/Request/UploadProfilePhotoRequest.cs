using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Request
{
    public class UploadProfilePhotoRequest
    {
        public IFormFile ProfileImage { get; set; } = default!;

    }
}
