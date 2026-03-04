using KolsheBjam3etna.DAL.Data;
using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.DTOs.Response.KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using KolsheBjam3etna.DAL.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Repository.Implementations
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _db;
        public EventRepository(ApplicationDbContext db) => _db = db;

        public async Task AddAsync(Event e) => await _db.Events.AddAsync(e);
        public async Task SaveAsync() => await _db.SaveChangesAsync();

        public async Task<List<CoordinatorEventCardDto>> GetCoordinatorEventsAsync(string coordinatorId)
        {
            return await _db.Events
                .AsNoTracking()
                .Include(x => x.Registrations)
                .Where(x => x.CoordinatorId == coordinatorId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new CoordinatorEventCardDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Type = x.Type,
                    Location = x.Location,
                    DateTimeUtc = x.DateTimeUtc,
                    Capacity = x.Capacity,
                    RegisteredCount = x.Registrations.Count,
                    ProgressPercent = x.Capacity == 0 ? 0 : (int)Math.Round((x.Registrations.Count * 100.0) / x.Capacity),
                    CoverImageUrl = x.CoverImageUrl
                })
                .ToListAsync();
        }

        public async Task<CoordinatorDashboardDto> GetCoordinatorDashboardAsync(string coordinatorId)
        {
            var events = await _db.Events
                .AsNoTracking()
                .Include(x => x.Registrations)
                .Where(x => x.CoordinatorId == coordinatorId)
                .ToListAsync();

            var activeEvents = events.Count(e => e.DateTimeUtc >= DateTime.UtcNow);
            var totalRegs = events.Sum(e => e.Registrations.Count);
            var totalCapacity = events.Sum(e => e.Capacity);

            var rate = totalCapacity == 0 ? 0 : (totalRegs * 100.0) / totalCapacity;

            return new CoordinatorDashboardDto
            {
                ActiveEventsCount = activeEvents,
                TotalRegistrations = totalRegs,
                RegistrationRatePercent = Math.Round(rate, 1),
                Performance = events
                    .OrderByDescending(e => e.CreatedAtUtc)
                    .Take(5)
                    .Select(e => new CoordinatorEventCardDto
                    {
                        Id = e.Id,
                        Title = e.Title,
                        Type = e.Type,
                        Location = e.Location,
                        DateTimeUtc = e.DateTimeUtc,
                        Capacity = e.Capacity,
                        RegisteredCount = e.Registrations.Count,
                        ProgressPercent = e.Capacity == 0 ? 0 : (int)Math.Round((e.Registrations.Count * 100.0) / e.Capacity),
                        CoverImageUrl = e.CoverImageUrl
                    })
                    .ToList()
            };
        }
        public async Task<List<EventRegistrantDto>> GetRegistrationsAsync(int eventId, string coordinatorId)
        {
            var exists = await _db.Events.AsNoTracking()
                .AnyAsync(e => e.Id == eventId && e.CoordinatorId == coordinatorId);

            if (!exists) return new List<EventRegistrantDto>();

            return await _db.EventRegistrations
                .AsNoTracking()
                .Include(r => r.User)
                .Where(r => r.EventId == eventId)
                .OrderByDescending(r => r.RegisteredAtUtc)
                .Select(r => new EventRegistrantDto
                {
                    UserId = r.UserId,
                    FullName = r.User.FullName,
                    ProfileImageUrl = r.User.ProfileImageUrl,
                    Major = r.User.Major,
                    StudyYear = r.User.StudyYear,
                    UniversityId = r.User.UniversityId,
                    RegisteredAtUtc = r.RegisteredAtUtc
                })
                .ToListAsync();
        }
        public async Task<bool> DeleteAsync(int eventId, string coordinatorId)
        {
            var ev = await _db.Events
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.Id == eventId && e.CoordinatorId == coordinatorId);

            if (ev == null) return false;

            _db.Events.Remove(ev); 
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateAsync(int eventId, string coordinatorId, UpdateEventRequest req, string? newCoverUrl)
        {
            var ev = await _db.Events.FirstOrDefaultAsync(e => e.Id == eventId && e.CoordinatorId == coordinatorId);
            if (ev == null) return false;

            ev.Title = req.Title.Trim();
            ev.Type = req.Type.Trim();
            ev.Location = req.Location.Trim();
            ev.DateTimeUtc = req.DateTimeUtc;
            ev.Capacity = req.Capacity;
            ev.Description = req.Description.Trim();

            if (!string.IsNullOrEmpty(newCoverUrl))
                ev.CoverImageUrl = newCoverUrl;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
