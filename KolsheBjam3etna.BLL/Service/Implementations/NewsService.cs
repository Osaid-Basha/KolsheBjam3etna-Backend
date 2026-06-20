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
        private readonly INotificationService _notificationService;

        public NewsService(
            INewsRepository repo,
            ILocalFileStorageService storage,
            INotificationService notificationService)
        {
            _repo = repo;
            _storage = storage;
            _notificationService = notificationService;
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
                description = string.IsNullOrWhiteSpace(req.description)
                    ? req.Content.Trim()
                    : req.description.Trim(),

            };

            await _repo.AddAsync(news);
            await _repo.SaveAsync();

            if (news.IsPublished && news.IsImportant)
                await SendImportantNewsNotificationAsync(news);

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

            var shouldNotify =
                req.IsImportant &&
                req.IsPublished &&
                (!news.IsImportant || !news.IsPublished);

            news.Title = req.Title.Trim();
            news.Content = req.Content.Trim();
            news.Source = req.Source.Trim();
            news.Category = req.Category.Trim();
            news.IsImportant = req.IsImportant;
            news.IsPublished = req.IsPublished;
            news.description = string.IsNullOrWhiteSpace(req.description)
                ? req.Content.Trim()
                : req.description.Trim();


            if (req.Image != null)
                news.ImageUrl = await _storage.SaveNewsImageAsync(req.Image);

            await _repo.SaveAsync();

            if (shouldNotify)
                await SendImportantNewsNotificationAsync(news);

            return ApiResponse<string>.Ok("Updated", "News updated successfully");
        }

        private async Task SendImportantNewsNotificationAsync(News news)
        {
            var body = news.description;

            if (string.IsNullOrWhiteSpace(body))
                body = news.Content;

            if (body.Length > 140)
                body = $"{body[..137]}...";

            await _notificationService.CreateForAllUsersAsync(
                "خبر مهم",
                body,
                "Announcement",
                targetType: "News",
                targetId: news.Id
            );
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
