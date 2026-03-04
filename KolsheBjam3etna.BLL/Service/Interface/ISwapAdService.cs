using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.BLL.Service.Interface
{
    public interface ISwapAdService
    {
        Task<ApiResponse<int>> CreateAsync(string userId, CreateSwapAdRequest request);
        Task<ApiResponse<List<SwapAdListDto>>> GetAllAsync(int? categoryId, string? search);
        Task<ApiResponse<SwapAdDetailsDto>> GetDetailsAsync(int id);
        Task<ApiResponse<List<SwapAdListDto>>> GetMineAsync(string userId);
    }
}
