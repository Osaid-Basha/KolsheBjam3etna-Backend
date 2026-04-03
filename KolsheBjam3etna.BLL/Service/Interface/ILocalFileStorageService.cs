using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.AspNetCore.Http;

namespace KolsheBjam3etna.BLL.Service.Interface
{
    public interface ILocalFileStorageService
    {
        Task<string?> SaveProfileImageAsync(IFormFile file);
        Task<string?> SaveChatImageAsync(IFormFile file);
        Task<string?> SaveChatFileAsync(IFormFile file);
        Task DeleteFileAsync(string? relativePath);
        Task<string?> SaveRequestFileAsync(IFormFile file);
        Task<string?> SaveEventCoverAsync(IFormFile file);
        Task<string?> SaveNewsImageAsync(IFormFile file);
        Task<string?> SavePartnerOfferImageAsync(IFormFile file);
    }
}
