using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using KolsheBjam3etna.DAL.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.BLL.Service.Implementations
{
    public class SwapAdService : ISwapAdService
    {
        private readonly ISwapAdRepository _repo;
        private readonly ILocalFileStorageService _storage;

        public SwapAdService(ISwapAdRepository repo, ILocalFileStorageService storage)
        {
            _repo = repo;
            _storage = storage;
        }

        public async Task<ApiResponse<int>> CreateAsync(string userId, CreateSwapAdRequest request)
        {
            if (!await _repo.CategoryExistsAsync(request.CategoryId))
                return new ApiResponse<int>(false, "Invalid category");

            if (request.Images != null && request.Images.Count > 5)
                return new ApiResponse<int>(false, "Max 5 images");

            var ad = new SwapAd
            {
                UserId = userId,
                OfferTitle = request.OfferTitle,
                WantedTitle = request.WantedTitle,
                CategoryId = request.CategoryId,
                Condition = request.Condition,
                Description = request.Description
            };

            if (request.Images != null)
            {
                foreach (var img in request.Images)
                {
                    var url = await _storage.SaveRequestFileAsync(img);

                    if (url == null) continue;

                    ad.Images.Add(new SwapAdImage
                    {
                        ImageUrl = url
                    });
                }
            }

            await _repo.AddAsync(ad);

            await _repo.SaveAsync();

            return new ApiResponse<int>(true, "Swap created", ad.Id);
        }
        public async Task<ApiResponse<List<SwapAdListDto>>> GetAllAsync(int? categoryId, string? search)
        {
            var items = await _repo.GetAllAsync(categoryId, search);
            return ApiResponse<List<SwapAdListDto>>.Ok(items, "Success");
        }

        public async Task<ApiResponse<SwapAdDetailsDto>> GetDetailsAsync(int id)
        {
            var dto = await _repo.GetDetailsAsync(id);
            if (dto == null) return ApiResponse<SwapAdDetailsDto>.Fail("Not found");
            return ApiResponse<SwapAdDetailsDto>.Ok(dto, "Success");
        }

        public async Task<ApiResponse<List<SwapAdListDto>>> GetMineAsync(string userId)
        {
            var items = await _repo.GetMineAsync(userId);
            return ApiResponse<List<SwapAdListDto>>.Ok(items, "Success");
        }
    }
    }
