using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using KolsheBjam3etna.DAL.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.BLL.Service.Implementations
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _repo;
        private readonly ILocalFileStorageService _storage;

        public NewsService(INewsRepository repo,
            ILocalFileStorageService storage)
        {
            _repo = repo;
            _storage = storage;
        }

        public async Task<ApiResponse<int>> CreateAsync(CreateNewsRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Title))
                return ApiResponse<int>.Fail("Title required");

            string? imageUrl = null;

            if (req.Image != null)
                imageUrl = await _storage.SaveNewsImageAsync(req.Image);

            var news = new News
            {
                Title = req.Title.Trim(),
                Content = req.Content.Trim(),
                Source = req.Source.Trim(),
                Category = req.Category.Trim(),
                ImageUrl = imageUrl,
                IsImportant = req.IsImportant,
                IsPublished = req.IsPublished
            };

            await _repo.AddAsync(news);
            await _repo.SaveAsync();

            return ApiResponse<int>.Ok(news.Id, "News created");
        }

        public async Task<ApiResponse<string>> UpdateAsync(int id, UpdateNewsRequest req)
        {
            var news = await _repo.GetByIdAsync(id);

            if (news == null)
                return ApiResponse<string>.Fail("News not found");

            news.Title = req.Title;
            news.Source = req.Source;
            news.Category = req.Category;
            news.Content = req.Content;
            news.IsImportant = req.IsImportant;

            if (req.Image != null)
                news.ImageUrl = await _storage.SaveNewsImageAsync(req.Image);

            await _repo.SaveAsync();

            return ApiResponse<string>.Ok("Updated", "News updated");
        }

        public async Task<ApiResponse<string>> PublishAsync(int id)
        {
            var news = await _repo.GetByIdAsync(id);

            if (news == null)
                return ApiResponse<string>.Fail("News not found");

            news.IsPublished = true;

            await _repo.SaveAsync();

            return ApiResponse<string>.Ok("Published", "News published");
        }

        public async Task<ApiResponse<List<NewsListItemDto>>> GetAdminListAsync()
        {
            var items = await _repo.GetAdminListAsync();

            return ApiResponse<List<NewsListItemDto>>.Ok(items, "Success");
        }

        public async Task<ApiResponse<List<NewsListItemDto>>> GetPublishedListAsync()
        {
            var items = await _repo.GetPublishedListAsync();

            return ApiResponse<List<NewsListItemDto>>.Ok(items, "Success");
        }

        public async Task<ApiResponse<News>> GetDetailsAsync(int id)
        {
            var news = await _repo.GetByIdAsync(id);

            if (news == null)
                return ApiResponse<News>.Fail("News not found");

            news.ViewsCount++;

            await _repo.SaveAsync();

            return ApiResponse<News>.Ok(news, "Success");
        }

        public async Task<ApiResponse<string>> UnpublishAsync(int id)
        {
            var news = await _repo.GetByIdAsync(id);

            if (news == null)
                return ApiResponse<string>.Fail("News not found");

            if (!news.IsPublished)
                return ApiResponse<string>.Fail("News already unpublished");

            news.IsPublished = false;

            await _repo.SaveAsync();

            return ApiResponse<string>.Ok("Unpublished", "News moved to draft");
        }
        public async Task<ApiResponse<string>> RemoveAsync(int id)
        {
            var news = await _repo.GetByIdAsync(id);
            if (news == null)
                return ApiResponse<string>.Fail("News not found");
            await _repo.Remove(id);
            await _repo.SaveAsync();
            return ApiResponse<string>.Ok("Removed", "News removed");
        }
    }
}
