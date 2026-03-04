using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Repository.Interface
{
    public interface ISwapAdRepository
    {
        Task<bool> CategoryExistsAsync(int id);

        Task AddAsync(SwapAd ad);

        Task SaveAsync();
        Task<List<SwapAdListDto>> GetAllAsync(int? categoryId, string? search);
        Task<SwapAdDetailsDto?> GetDetailsAsync(int id);
        Task<List<SwapAdListDto>> GetMineAsync(string userId);

    }
}
