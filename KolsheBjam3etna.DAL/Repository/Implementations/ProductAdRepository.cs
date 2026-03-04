using KolsheBjam3etna.DAL.Data;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using KolsheBjam3etna.DAL.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Repository.Implementations
{
    public class ProductAdRepository : IProductAdRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductAdRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<bool> CategoryExistsAsync(int id)
        {
            return _db.ProductCategories.AnyAsync(x => x.Id == id);
        }

        public async Task AddAsync(ProductAd ad)
        {
            await _db.ProductAds.AddAsync(ad);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
        public async Task<List<ProductAdListDto>> GetAllAsync(int? categoryId, string? search)
        {
            var q = _db.ProductAds
                .AsNoTracking()
                .Include(x => x.User)     // ✅
                .Include(x => x.Category)
                .Include(x => x.Images)
                .AsQueryable();

            if (categoryId.HasValue)
                q = q.Where(x => x.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(x => x.Title.Contains(search) || x.Description.Contains(search));

            return await q
                .OrderByDescending(x => x.CreatedAtUtc) 
                .Select(x => new ProductAdListDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Price = x.Price,
                    Condition = x.Condition.ToString(),

                    CategoryId = x.CategoryId,
                    CategoryName = x.Category.Name,

                    UserId = x.UserId,
                    User = new UserMiniDto
                    {
                        Id = x.User.Id,
                        FullName = x.User.FullName,
                        ProfileImageUrl = x.User.ProfileImageUrl,
                        Major = x.User.Major,
                        StudyYear = x.User.StudyYear,
                        UniversityId = x.User.UniversityId
                    },

                    CoverImageUrl = x.Images.Select(i => i.ImageUrl).FirstOrDefault(),
                    CreatedAtUtc = x.CreatedAtUtc
                })
                .ToListAsync();
        }

        public async Task<ProductAdDetailsDto?> GetDetailsAsync(int id)
        {
            return await _db.ProductAds
                .AsNoTracking()
                .Include(x => x.User)     // ✅
                .Include(x => x.Category)
                .Include(x => x.Images)
                .Where(x => x.Id == id)
                .Select(x => new ProductAdDetailsDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Price = x.Price,
                    Condition = x.Condition.ToString(),
                    Description = x.Description,

                    CategoryId = x.CategoryId,
                    CategoryName = x.Category.Name,

                    UserId = x.UserId,
                    User = new UserMiniDto
                    {
                        Id = x.User.Id,
                        FullName = x.User.FullName,
                        ProfileImageUrl = x.User.ProfileImageUrl,
                        Major = x.User.Major,
                        StudyYear = x.User.StudyYear,
                        UniversityId = x.User.UniversityId
                    },

                    CreatedAtUtc = x.CreatedAtUtc,
                    Images = x.Images.Select(i => i.ImageUrl).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<ProductAdListDto>> GetMineAsync(string userId)
        {
            var q = _db.ProductAds
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.Category)
                .Include(x => x.Images)
                .Where(x => x.UserId == userId);

            return await q
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new ProductAdListDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Price = x.Price,
                    Condition = x.Condition.ToString(),
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category.Name,

                    UserId = x.UserId,
                    User = new UserMiniDto
                    {
                        Id = x.User.Id,
                        FullName = x.User.FullName,
                        ProfileImageUrl = x.User.ProfileImageUrl,
                        Major = x.User.Major,
                        StudyYear = x.User.StudyYear,
                        UniversityId = x.User.UniversityId
                    },

                    CoverImageUrl = x.Images.Select(i => i.ImageUrl).FirstOrDefault(),
                    CreatedAtUtc = x.CreatedAtUtc
                })
                .ToListAsync();
        }

    }
}
