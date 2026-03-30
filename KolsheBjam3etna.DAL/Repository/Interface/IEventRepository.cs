using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.DTOs.Response.KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Repository.Interface
{
    public interface IEventRepository
    {
        Task AddAsync(Event e);
        Task SaveAsync();

        Task<List<CoordinatorEventCardDto>> GetCoordinatorEventsAsync(string coordinatorId);
        Task<CoordinatorDashboardDto> GetCoordinatorDashboardAsync(string coordinatorId);
        Task<List<EventRegistrantDto>> GetRegistrationsAsync(int eventId, string coordinatorId);
        Task<bool> DeleteAsync(int eventId, string coordinatorId);
        Task<bool> UpdateAsync(int eventId, string coordinatorId, UpdateEventRequest req, string? newCoverUrl);
        Task<Club?> GetClubByOwnerIdAsync(string ownerId);
    }
}
