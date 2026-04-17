using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using KolsheBjam3etna.DAL.Repository.Interface;

namespace KolsheBjam3etna.BLL.Service.Implementations
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _repo;
        private readonly ILocalFileStorageService _storage;

        public NewsService(
            INewsRepository repo,
            ILocalFileStorageService storage)
        {
            _repo = repo;
            _storage = storage;
        }

        public async Task<ApiResponse<int>> CreateAsync(CreateNewsRequest req)
        {
            if (req == null)
                return ApiResponse<int>.Fail("Invalid request");

            if (string.IsNullOrWhiteSpace(req.Title))
                return ApiResponse<int>.Fail("Title is required");

            if (string.IsNullOrWhiteSpace(req.Content))
                return ApiResponse<int>.Fail("Content is required");

            if (string.IsNullOrWhiteSpace(req.Source))
                return ApiResponse<int>.Fail("Source is required");

            if (string.IsNullOrWhiteSpace(req.Category))
                return ApiResponse<int>.Fail("Category is required");

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
                IsPublished = req.IsPublished,
                description = req.description.Trim(),

            };

            await _repo.AddAsync(news);
            await _repo.SaveAsync();

            return ApiResponse<int>.Ok(news.Id, "News created successfully");
        }

        public async Task<ApiResponse<string>> UpdateAsync(int id, UpdateNewsRequest req)
        {
            if (req == null)
                return ApiResponse<string>.Fail("Invalid request");

            var news = await _repo.GetByIdAsync(id);

            if (news == null)
                return ApiResponse<string>.Fail("News not found");

            if (string.IsNullOrWhiteSpace(req.Title))
                return ApiResponse<string>.Fail("Title is required");

            if (string.IsNullOrWhiteSpace(req.Content))
                return ApiResponse<string>.Fail("Content is required");

            if (string.IsNullOrWhiteSpace(req.Source))
                return ApiResponse<string>.Fail("Source is required");

            if (string.IsNullOrWhiteSpace(req.Category))
                return ApiResponse<string>.Fail("Category is required");

            news.Title = req.Title.Trim();
            news.Content = req.Content.Trim();
            news.Source = req.Source.Trim();
            news.Category = req.Category.Trim();
            news.IsImportant = req.IsImportant;
            news.description = req.description;


            if (req.Image != null)
                news.ImageUrl = await _storage.SaveNewsImageAsync(req.Image);

            await _repo.SaveAsync();

            return ApiResponse<string>.Ok("Updated", "News updated successfully");
        }

     

        public async Task<ApiResponse<string>> RemoveAsync(int id)
        {
            var news = await _repo.GetByIdAsync(id);

            if (news == null)
                return ApiResponse<string>.Fail("News not found");

            await _repo.Remove(news);
            await _repo.SaveAsync();

            return ApiResponse<string>.Ok("Removed", "News deleted successfully");
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

        public async Task<ApiResponse<NewsAdminDetailsDto>> GetAdminDetailsAsync(int id)
        {
            var news = await _repo.GetByIdAsync(id);

            if (news == null)
                return ApiResponse<NewsAdminDetailsDto>.Fail("News not found");

            var dto = new NewsAdminDetailsDto
            {
                Id = news.Id,
                Title = news.Title,
                Content = news.Content,
                Source = news.Source,
                Category = news.Category,
                ImageUrl = news.ImageUrl,
                IsImportant = news.IsImportant,
                IsPublished = news.IsPublished,
                ViewsCount = news.ViewsCount,
                CreatedAtUtc = news.CreatedAtUtc,
                description = news.description


            };

            return ApiResponse<NewsAdminDetailsDto>.Ok(dto, "Success");
        }

        public async Task<ApiResponse<News>> GetDetailsAsync(int id)
        {
            var news = await _repo.GetByIdAsync(id);

            if (news == null || !news.IsPublished)
                return ApiResponse<News>.Fail("News not found");

            news.ViewsCount++;
            await _repo.SaveAsync();

            return ApiResponse<News>.Ok(news, "Success");
        }
    }
}