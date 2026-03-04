using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.BLL.Service.Implementations
{
    public class EventPublicService : IEventPublicService
    {
        private readonly IEventPublicRepository _repo;

        public EventPublicService(IEventPublicRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<List<EventListItemDto>>> GetAllAsync(string? search, string? type)
        {
            var items = await _repo.GetAllAsync(search, type);
            return ApiResponse<List<EventListItemDto>>.Ok(items, "Success");
        }

        public async Task<ApiResponse<EventDetailsDto>> GetDetailsAsync(int eventId)
        {
            var dto = await _repo.GetDetailsAsync(eventId);
            if (dto == null) return ApiResponse<EventDetailsDto>.Fail("Event not found");
            return ApiResponse<EventDetailsDto>.Ok(dto, "Success");
        }

        public async Task<ApiResponse<int>> RegisterAsync(int eventId, string userId, RegisterEventRequest req)
        {
            if (!await _repo.IsEventExistsAsync(eventId))
                return ApiResponse<int>.Fail("Event not found");

            if (await _repo.IsUserRegisteredAsync(eventId, userId))
                return ApiResponse<int>.Fail("Already registered");

            if (string.IsNullOrWhiteSpace(req.FullName))
                return ApiResponse<int>.Fail("FullName is required");

            if (string.IsNullOrWhiteSpace(req.UniversityEmail))
                return ApiResponse<int>.Fail("UniversityEmail is required");

            if (req.StudyYear < 1 || req.StudyYear > 4)
                return ApiResponse<int>.Fail("StudyYear must be 1..4");

           
            var count = await _repo.GetRegistrationsCountAsync(eventId);
          

            var id = await _repo.RegisterAsync(eventId, userId, req);
            return ApiResponse<int>.Ok(id, "Registered successfully");
        }
    }
}
