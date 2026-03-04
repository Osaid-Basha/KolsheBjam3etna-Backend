using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.DTOs.Response.KolsheBjam3etna.DAL.DTOs.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.BLL.Service.Interface
{
    public interface IEventService
    {
        Task<ApiResponse<int>> CreateAsync(string coordinatorId, CreateEventRequest req);
        Task<ApiResponse<List<CoordinatorEventCardDto>>> GetMyEventsAsync(string coordinatorId);
        Task<ApiResponse<CoordinatorDashboardDto>> GetDashboardAsync(string coordinatorId);
        Task<ApiResponse<List<EventRegistrantDto>>> GetRegistrationsAsync(string coordinatorId, int eventId);
        Task<ApiResponse<string>> DeleteAsync(string coordinatorId, int eventId);
        Task<ApiResponse<string>> UpdateAsync(string coordinatorId, int eventId, UpdateEventRequest req);

    }
}
