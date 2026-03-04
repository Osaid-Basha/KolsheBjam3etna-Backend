using KolsheBjam3etna.BLL.Service.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.BLL.Service.Class
{
    public class LocalFileStorageService: ILocalFileStorageService
    {
        private readonly IWebHostEnvironment _env;
        public LocalFileStorageService(IWebHostEnvironment env) => _env = env;

        public async Task<string?> SaveProfileImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowed.Contains(ext)) throw new Exception("Invalid image type");

            var folder = Path.Combine(_env.WebRootPath, "uploads", "profiles");
            Directory.CreateDirectory(folder);

            var name = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(folder, name);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/profiles/{name}";
        }
    }
}
