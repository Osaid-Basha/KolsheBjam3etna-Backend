using KolsheBjam3etna.BLL.Service.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace KolsheBjam3etna.BLL.Service.Class
{
    public class LocalFileStorageService : ILocalFileStorageService
    {
        private readonly IWebHostEnvironment _env;

        public LocalFileStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string?> SaveProfileImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

          
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowed.Contains(ext))
                throw new Exception("Invalid image type");

           
            if (file.Length > 5 * 1024 * 1024)
                throw new Exception("File size must be less than 5MB");

            
            var wwwroot = _env.WebRootPath
                ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var folder = Path.Combine(wwwroot, "uploads", "profiles");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

         
            var fileName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(folder, fileName);

         
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

         
            return $"/uploads/profiles/{fileName}";
        }
        public async Task<string?> SaveChatImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowed.Contains(ext)) throw new Exception("Invalid image type");

          
            if (file.Length > 5 * 1024 * 1024) throw new Exception("Image too large");

            var wwwroot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var folder = Path.Combine(wwwroot, "uploads", "chat");
            Directory.CreateDirectory(folder);

            var name = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(folder, name);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/chat/{name}";
        }
        public async Task<string?> SaveRequestFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".pdf" };
            if (!allowed.Contains(ext))
                throw new Exception("Invalid file type");

            var wwwroot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var folder = Path.Combine(wwwroot, "uploads", "requests");
            Directory.CreateDirectory(folder);

            var name = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(folder, name);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/requests/{name}";
        }
        public async Task<string?> SaveEventCoverAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowedExt = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowedExt.Contains(ext))
                throw new Exception("Invalid image type");

            var wwwroot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var folder = Path.Combine(wwwroot, "uploads", "events");
            Directory.CreateDirectory(folder);

            var name = $"{Guid.NewGuid()}{ext}";
            var path = Path.Combine(folder, name);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/events/{name}";
        }

        public Task<string?> SaveNewsImageAsync(IFormFile file)
        {

            if (file == null || file.Length == 0) return null;
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowed.Contains(ext)) throw new Exception("Invalid image type");
            if (file.Length > 5 * 1024 * 1024) throw new Exception("Image too large");
            var wwwroot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var folder = Path.Combine(wwwroot, "uploads", "news");
            Directory.CreateDirectory(folder);
            var name = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(folder, name);
            using var stream = new FileStream(fullPath, FileMode.Create);
            file.CopyTo(stream);
            return Task.FromResult<string?>($"/uploads/news/{name}");

        }
        public async Task<string?> SavePartnerOfferImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowed.Contains(ext))
                throw new Exception("Invalid image type");

            var wwwroot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var folder = Path.Combine(wwwroot, "uploads", "partner-offers");
            Directory.CreateDirectory(folder);

            var name = $"{Guid.NewGuid()}{ext}";
            var path = Path.Combine(folder, name);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/partner-offers/{name}";
        }
    }
}