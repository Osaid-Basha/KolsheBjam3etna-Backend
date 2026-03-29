using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;

namespace KolsheBjam3etna.BLL.Service.Interface
{
    public interface INewsService
    {
        Task<ApiResponse<int>> CreateAsync(CreateNewsRequest req);
        Task<ApiResponse<string>> UpdateAsync(int id, UpdateNewsRequest req);
  
        Task<ApiResponse<string>> RemoveAsync(int id);

        Task<ApiResponse<List<NewsListItemDto>>> GetAdminListAsync();
        Task<ApiResponse<List<NewsListItemDto>>> GetPublishedListAsync();

        Task<ApiResponse<NewsAdminDetailsDto>> GetAdminDetailsAsync(int id);
        Task<ApiResponse<News>> GetDetailsAsync(int id);
    }
}