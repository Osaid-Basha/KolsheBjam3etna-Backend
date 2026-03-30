using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;

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
        Task<EventBasicDto?> GetEventBasicAsync(int eventId);
    }
}