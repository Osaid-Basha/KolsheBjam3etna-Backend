using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.BLL.Service.Interface
{
    public interface INewsService
    {
        Task<ApiResponse<int>> CreateAsync(CreateNewsRequest req);

        Task<ApiResponse<string>> UpdateAsync(int id, UpdateNewsRequest req);

        Task<ApiResponse<string>> PublishAsync(int id);

        Task<ApiResponse<List<NewsListItemDto>>> GetAdminListAsync();

        Task<ApiResponse<List<NewsListItemDto>>> GetPublishedListAsync();

        Task<ApiResponse<News>> GetDetailsAsync(int id);

        Task<ApiResponse<string>> UnpublishAsync(int id);

        Task<ApiResponse<string>> RemoveAsync(int id);
    }
}
