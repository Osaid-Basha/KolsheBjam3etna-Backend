using KolsheBjam3etna.DAL.Data;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using KolsheBjam3etna.DAL.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace KolsheBjam3etna.DAL.Repository.Implementations
{
    public class NewsRepository : INewsRepository
    {
        private readonly ApplicationDbContext _db;

        public NewsRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(News news)
            => await _db.News.AddAsync(news);

        public async Task SaveAsync()
            => await _db.SaveChangesAsync();

        public async Task<News?> GetByIdAsync(int id)
            => await _db.News.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<List<NewsListItemDto>> GetAdminListAsync()
        {
            return await _db.News
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new NewsListItemDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Source = x.Source,
                    Category = x.Category,
                    ImageUrl = x.ImageUrl,
                    IsImportant = x.IsImportant,
                    ViewsCount = x.ViewsCount,
                    CreatedAtUtc = x.CreatedAtUtc
                })
                .ToListAsync();
        }

        public async Task<List<NewsListItemDto>> GetPublishedListAsync()
        {
            return await _db.News
                .AsNoTracking()
                .Where(x => x.IsPublished)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new NewsListItemDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Source = x.Source,
                    Category = x.Category,
                    ImageUrl = x.ImageUrl,
                    IsImportant = x.IsImportant,
                    ViewsCount = x.ViewsCount,
                    CreatedAtUtc = x.CreatedAtUtc
                })
                .ToListAsync();
        }
        public async Task Remove(int id)
        {
            var news = await _db.News.FirstOrDefaultAsync(x => x.Id == id);
            if (news != null)
            {
                _db.News.Remove(news);
                await _db.SaveChangesAsync();
            }
        }
    }
}
