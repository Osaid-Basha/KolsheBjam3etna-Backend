using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;

namespace KolsheBjam3etna.DAL.Repository.Interface
{
    public interface INewsRepository
    {
        Task AddAsync(News news);
        Task SaveAsync();
        Task<News?> GetByIdAsync(int id);
        Task<List<NewsListItemDto>> GetAdminListAsync();
        Task<List<NewsListItemDto>> GetPublishedListAsync();
        Task Remove(News news);
    }
}