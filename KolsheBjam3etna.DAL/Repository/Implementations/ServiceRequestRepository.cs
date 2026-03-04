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
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        private readonly ApplicationDbContext _db;

        public ServiceRequestRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<bool> CategoryExistsAsync(int categoryId)
            => _db.ServiceCategories.AnyAsync(x => x.Id == categoryId);

        public async Task AddAsync(ServiceRequest request)
            => await _db.ServiceRequests.AddAsync(request);

        public Task SaveChangesAsync()
            => _db.SaveChangesAsync();


        public async Task<List<ServiceRequestListItemDto>> GetAllAsync(int? categoryId, string? search)
        {
            var q = _db.ServiceRequests
       .AsNoTracking()
       .Include(x => x.User)        // ✅ المهم
       .Include(x => x.Category)
       .Include(x => x.Attachments)
       .AsQueryable();

            if (categoryId.HasValue)
                q = q.Where(x => x.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(x => x.Title.Contains(search) || x.Description.Contains(search));

            return await q
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new ServiceRequestListItemDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Budget = x.Budget,
                    DeadlineUtc = x.DeadlineUtc,
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
                    AttachmentsCount = x.Attachments.Count
                })
                .ToListAsync();
        }

        public async Task<ServiceRequestDetailsDto?> GetDetailsAsync(int id)
        {
            return await _db.ServiceRequests
                .AsNoTracking()
                .Include(x => x.User)        
                .Include(x => x.Category)
                .Include(x => x.Attachments)
                .Where(x => x.Id == id)
                .Select(x => new ServiceRequestDetailsDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Budget = x.Budget,
                    DeadlineUtc = x.DeadlineUtc,
                    Description = x.Description,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category.Name,
                    UserId = x.UserId,
                    CreatedAtUtc = x.CreatedAtUtc,

                  
                    User = new UserMiniDto
                    {
                        Id = x.User.Id,
                        FullName = x.User.FullName,
                        ProfileImageUrl = x.User.ProfileImageUrl,
                        Major = x.User.Major,
                        StudyYear = x.User.StudyYear,
                        UniversityId = x.User.UniversityId
                    },

                    Attachments = x.Attachments.Select(a => new ServiceRequestAttachmentDto
                    {
                        Id = a.Id,
                        FileUrl = a.FileUrl,
                        FileName = a.FileName,
                        Size = a.Size,
                        UploadedAtUtc = a.UploadedAtUtc
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<ServiceRequestListItemDto>> GetMineAsync(string userId)
        {
            return await _db.ServiceRequests
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.Attachments)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new ServiceRequestListItemDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Budget = x.Budget,
                    Description = x.Description,
                    DeadlineUtc = x.DeadlineUtc,
                    CategoryName = x.Category.Name,
                    CreatedAtUtc = x.CreatedAtUtc,
                    AttachmentsCount = x.Attachments.Count
                })
                .ToListAsync();
        }
    }
}