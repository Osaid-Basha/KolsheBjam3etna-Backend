using KolsheBjam3etna.DAL.Data;
using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using KolsheBjam3etna.DAL.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace KolsheBjam3etna.DAL.Repository.Implementations
{
    public class EventPublicRepository : IEventPublicRepository
    {
        private readonly ApplicationDbContext _db;
        public EventPublicRepository(ApplicationDbContext db) => _db = db;

        public async Task<List<EventListItemDto>> GetAllAsync(string? search, string? type)
        {
            var q = _db.Events.AsNoTracking()
                .Include(e => e.Registrations)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(e => e.Title.Contains(search) || e.Description.Contains(search));

            if (!string.IsNullOrWhiteSpace(type))
                q = q.Where(e => e.Type == type);

            return await q
                .OrderBy(e => e.DateTimeUtc)
                .Select(e => new EventListItemDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Type = e.Type,
                    Location = e.Location,
                    DateTimeUtc = e.DateTimeUtc,
                    Capacity = e.Capacity,
                    RegisteredCount = e.Registrations.Count,
                    CoverImageUrl = e.CoverImageUrl
                })
                .ToListAsync();
        }

        public async Task<EventDetailsDto?> GetDetailsAsync(int eventId)
        {
            return await _db.Events.AsNoTracking()
                .Include(e => e.Registrations)
                .Include(e => e.Coordinator)
                .Where(e => e.Id == eventId)
                .Select(e => new EventDetailsDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Type = e.Type,
                    Location = e.Location,
                    DateTimeUtc = e.DateTimeUtc,
                    Capacity = e.Capacity,
                    RegisteredCount = e.Registrations.Count,
                    Description = e.Description,
                    CoverImageUrl = e.CoverImageUrl,

                    CoordinatorId = e.CoordinatorId,
                    CoordinatorName = e.Coordinator.FullName,
                    CoordinatorProfileImageUrl = e.Coordinator.ProfileImageUrl
                })
                .FirstOrDefaultAsync();
        }

        public Task<bool> IsEventExistsAsync(int eventId)
            => _db.Events.AsNoTracking().AnyAsync(e => e.Id == eventId);

        public Task<int> GetRegistrationsCountAsync(int eventId)
            => _db.EventRegistrations.CountAsync(r => r.EventId == eventId);

        public Task<bool> IsUserRegisteredAsync(int eventId, string userId)
            => _db.EventRegistrations.AnyAsync(r => r.EventId == eventId && r.UserId == userId);

        public async Task<int> RegisterAsync(int eventId, string userId, RegisterEventRequest req)
        {
            var reg = new EventRegistration
            {
                EventId = eventId,
                UserId = userId,
                FullName = req.FullName.Trim(),
                UniversityEmail = req.UniversityEmail.Trim(),
                StudyYear = req.StudyYear
            };

            _db.EventRegistrations.Add(reg);
            await _db.SaveChangesAsync();
            return reg.Id;
        }
    }
}
