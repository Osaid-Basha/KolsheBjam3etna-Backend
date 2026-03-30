using KolsheBjam3etna.DAL.Data;
using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using KolsheBjam3etna.DAL.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace KolsheBjam3etna.DAL.Repository.Implementations
{
    public class EventPublicRepository : IEventPublicRepository
    {
        private readonly ApplicationDbContext _db;

        public EventPublicRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<EventListItemDto>> GetAllAsync(string? search, string? type)
        {
            var q = _db.Events
                .AsNoTracking()
                .Include(e => e.Registrations)
                .Include(e => e.Club)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim();

                q = q.Where(e =>
                    e.Title.Contains(keyword) ||
                    e.Description.Contains(keyword) ||
                    e.Club.Name.Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                q = q.Where(e => e.Type == type);
            }

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
                    CoverImageUrl = e.CoverImageUrl,
                    Description = e.Description,
                    ClubName = e.Club.Name
                })
                .ToListAsync();
        }

        public async Task<EventDetailsDto?> GetDetailsAsync(int eventId)
        {
            return await _db.Events
                .AsNoTracking()
                .Include(e => e.Agenda)
                .Include(e => e.Registrations)
                .Include(e => e.Club)
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
                    Content = e.Content,
                    CoverImageUrl = e.CoverImageUrl,
                    ClubName = e.Club.Name,

                    Agenda = e.Agenda
                        .OrderBy(a => a.Order)
                        .Select(a => new EventAgendaItemDto
                        {
                            Id = a.Id,
                            Title = a.Title,
                            StartTime = a.StartTime.HasValue
                                ? a.StartTime.Value.ToString(@"hh\:mm")
                                : null,
                            Order = a.Order,
                            IsVisible = a.IsVisible
                        })
                        .ToList()
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

        public async Task<EventBasicDto?> GetEventBasicAsync(int eventId)
        {
            return await _db.Events
                .AsNoTracking()
                .Include(e => e.Club)
                .Where(e => e.Id == eventId)
                .Select(e => new EventBasicDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Capacity = e.Capacity,
                    ClubId = e.ClubId,
                    ClubOwnerId = e.Club != null ? e.Club.OwnerId : null
                })
                .FirstOrDefaultAsync();
        }
    }
}