using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.BLL.Service.Interface
{
    public interface IEventPublicService
    {
        Task<ApiResponse<List<EventListItemDto>>> GetAllAsync(string? search, string? type);
        Task<ApiResponse<EventDetailsDto>> GetDetailsAsync(int eventId);
        Task<ApiResponse<int>> RegisterAsync(int eventId, string userId, RegisterEventRequest req);
    }
}
