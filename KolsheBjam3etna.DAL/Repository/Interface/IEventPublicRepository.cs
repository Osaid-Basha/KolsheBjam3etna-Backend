using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Repository.Interface
{
    public interface IEventPublicRepository
    {
        Task<List<EventListItemDto>> GetAllAsync(string? search, string? type);
        Task<EventDetailsDto?> GetDetailsAsync(int eventId);

        Task<bool> IsEventExistsAsync(int eventId);
        Task<int> GetRegistrationsCountAsync(int eventId);
        Task<bool> IsUserRegisteredAsync(int eventId, string userId);

        Task<int> RegisterAsync(int eventId, string userId, RegisterEventRequest req);
        Task<(string Title, string CoordinatorId)?> GetEventBasicAsync(int eventId);
    }
}
