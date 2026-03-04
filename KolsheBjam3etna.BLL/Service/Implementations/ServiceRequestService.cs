using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.Data;
using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using KolsheBjam3etna.DAL.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
namespace KolsheBjam3etna.BLL.Service.Implementations
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly IServiceRequestRepository _repo;
        private readonly ILocalFileStorageService _storage;

        public ServiceRequestService(IServiceRequestRepository repo, ILocalFileStorageService storage)
        {
            _repo = repo;
            _storage = storage;
        }

        public async Task<ApiResponse<int>> CreateAsync(string userId, CreateServiceRequestRequest request)
        {
            if (!await _repo.CategoryExistsAsync(request.CategoryId))
                return ApiResponse<int>.Fail("Invalid categoryId");

            if (request.Files != null && request.Files.Count > 4)
                return ApiResponse<int>.Fail("Max files is 4");

            var entity = new ServiceRequest
            {
                UserId = userId,
                Title = request.Title.Trim(),
                CategoryId = request.CategoryId,
                Budget = request.Budget,
                DeadlineUtc = request.DeadlineUtc,
                Description = request.Description.Trim()
            };

            if (request.Files != null)
            {
                foreach (var f in request.Files)
                {
                    var url = await _storage.SaveRequestFileAsync(f);
                    if (url == null) continue;

                    entity.Attachments.Add(new ServiceRequestAttachment
                    {
                        FileUrl = url,
                        FileName = f.FileName,
                        Size = f.Length
                    });
                }
            }

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return ApiResponse<int>.Ok(entity.Id, "Request created");
        }
        public async Task<ApiResponse<List<ServiceRequestListItemDto>>> GetAllAsync(int? categoryId, string? search)
        {
            var items = await _repo.GetAllAsync(categoryId, search);
            return ApiResponse<List<ServiceRequestListItemDto>>.Ok(items, "Success");
        }

        public async Task<ApiResponse<ServiceRequestDetailsDto>> GetDetailsAsync(int id)
        {
            var dto = await _repo.GetDetailsAsync(id);
            if (dto == null) return ApiResponse<ServiceRequestDetailsDto>.Fail("Not found");
            return ApiResponse<ServiceRequestDetailsDto>.Ok(dto, "Success");
        }

        public async Task<ApiResponse<List<ServiceRequestListItemDto>>> GetMineAsync(string userId)
        {
            var items = await _repo.GetMineAsync(userId);
            return ApiResponse<List<ServiceRequestListItemDto>>.Ok(items, "Success");
        }
    }
}

