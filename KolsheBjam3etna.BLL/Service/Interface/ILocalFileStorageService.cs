using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.BLL.Service.Interface
{
    public interface ILocalFileStorageService
    {
        Task<string?> SaveProfileImageAsync(IFormFile file);
        
    }
}
