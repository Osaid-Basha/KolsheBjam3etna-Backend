using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.DTOs.Response.KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using KolsheBjam3etna.DAL.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Linq;

namespace KolsheBjam3etna.BLL.Service.Implementations
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _repo;
        private readonly ILocalFileStorageService _storage;
        private readonly INotificationService _notificationService; 

        public EventService(IEventRepository repo, ILocalFileStorageService storage, INotificationService notificationService)
        {
            _repo = repo;
            _storage = storage;
            _notificationService = notificationService;
        }

        public async Task<ApiResponse<int>> CreateAsync(string coordinatorId, CreateEventRequest req)
        {
            
            if (string.IsNullOrWhiteSpace(req.Title))
                return ApiResponse<int>.Fail("Title is required");

            if (req.Title.Length > 80)
                return ApiResponse<int>.Fail("Title max 80 chars");

            if (req.Capacity <= 0)
                return ApiResponse<int>.Fail("Capacity must be > 0");

            if (string.IsNullOrWhiteSpace(req.Type))
                return ApiResponse<int>.Fail("Type is required");

            if (string.IsNullOrWhiteSpace(req.Location))
                return ApiResponse<int>.Fail("Location is required");

            if (string.IsNullOrWhiteSpace(req.Description))
                return ApiResponse<int>.Fail("Description is required");

            if (req.Description.Length > 500)
                return ApiResponse<int>.Fail("Description max 500 chars");

            List<AgendaItemRequest> agenda = new();

            if (!string.IsNullOrWhiteSpace(req.AgendaJson))
            {
                try
                {
                    agenda = JsonSerializer.Deserialize<List<AgendaItemRequest>>(req.AgendaJson) ?? new();
                }
                catch
                {
                    return ApiResponse<int>.Fail("Invalid AgendaJson");
                }
            }

            string? coverUrl = null;
            if (req.CoverImage != null)
                coverUrl = await _storage.SaveEventCoverAsync(req.CoverImage);


            var e = new Event
            {
                CoordinatorId = coordinatorId,
                Title = req.Title.Trim(),
                Type = req.Type.Trim(),
                Location = req.Location.Trim(),
                DateTimeUtc = req.DateTimeUtc,
                Capacity = req.Capacity,
                Description = req.Description.Trim(),
                Content = string.IsNullOrWhiteSpace(req.Content) ? null : req.Content.Trim(),
                CoverImageUrl = coverUrl
            };
            foreach (var item in agenda.OrderBy(x => x.Order))
            {
                if (string.IsNullOrWhiteSpace(item.Title)) continue;

                TimeSpan? st = null;
                if (!string.IsNullOrWhiteSpace(item.StartTime) && TimeSpan.TryParse(item.StartTime, out var ts))
                    st = ts;

                e.Agenda.Add(new EventAgendaItem
                {
                    Title = item.Title.Trim(),
                    StartTime = st,
                    Order = item.Order,
                    IsVisible = item.IsVisible
                });
            }

            await _repo.AddAsync(e);
            await _repo.SaveAsync();

            return ApiResponse<int>.Ok(e.Id, "Event created");
        }

        public async Task<ApiResponse<List<CoordinatorEventCardDto>>> GetMyEventsAsync(string coordinatorId)
        {
            var items = await _repo.GetCoordinatorEventsAsync(coordinatorId);
            return ApiResponse<List<CoordinatorEventCardDto>>.Ok(items, "Success");
        }

        public async Task<ApiResponse<CoordinatorDashboardDto>> GetDashboardAsync(string coordinatorId)
        {
            var dto = await _repo.GetCoordinatorDashboardAsync(coordinatorId);
            return ApiResponse<CoordinatorDashboardDto>.Ok(dto, "Success");
        }
        public async Task<ApiResponse<List<EventRegistrantDto>>> GetRegistrationsAsync(string coordinatorId, int eventId)
        {
            var items = await _repo.GetRegistrationsAsync(eventId, coordinatorId);

           
            if (items.Count == 0)
                return ApiResponse<List<EventRegistrantDto>>.Ok(items, "No registrations or not found");
       

            return ApiResponse<List<EventRegistrantDto>>.Ok(items, "Success");
        }
        public async Task<ApiResponse<string>> DeleteAsync(string coordinatorId, int eventId)
        {
            var ok = await _repo.DeleteAsync(eventId, coordinatorId);
            if (!ok) return ApiResponse<string>.Fail("Event not found");
            return ApiResponse<string>.Ok("Deleted", "Event deleted");
        }
        public async Task<ApiResponse<string>> UpdateAsync(string coordinatorId, int eventId, UpdateEventRequest req)
        {
            string? coverUrl = null;
            if (req.CoverImage != null)
                coverUrl = await _storage.SaveEventCoverAsync(req.CoverImage);

            var ok = await _repo.UpdateAsync(eventId, coordinatorId, req, coverUrl);
            if (!ok) return ApiResponse<string>.Fail("Event not found");

            return ApiResponse<string>.Ok("Updated", "Event updated");
        }

    }
    }
