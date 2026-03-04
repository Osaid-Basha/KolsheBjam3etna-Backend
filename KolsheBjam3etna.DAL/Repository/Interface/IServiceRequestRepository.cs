using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Repository.Interface
{
    public interface IServiceRequestRepository
    {
        Task<bool> CategoryExistsAsync(int categoryId);
        Task AddAsync(ServiceRequest request);
        Task SaveChangesAsync();
        Task<List<ServiceRequestListItemDto>> GetAllAsync(int? categoryId, string? search);
        Task<ServiceRequestDetailsDto?> GetDetailsAsync(int id);
        Task<List<ServiceRequestListItemDto>> GetMineAsync(string userId);

    }
}
