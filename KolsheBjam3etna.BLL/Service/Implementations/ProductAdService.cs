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
    public class ProductAdService : IProductAdService
    {
        private readonly IProductAdRepository _repo;
        private readonly ILocalFileStorageService _storage;

        public ProductAdService(IProductAdRepository repo, ILocalFileStorageService storage)
        {
            _repo = repo;
            _storage = storage;
        }

        public async Task<ApiResponse<int>> CreateAsync(string userId, CreateProductAdRequest request)
        {
            if (!await _repo.CategoryExistsAsync(request.CategoryId))
                return new ApiResponse<int>(false, "Invalid category");

            if (request.Images != null && request.Images.Count > 5)
                return new ApiResponse<int>(false, "Max 5 images");

            var ad = new ProductAd
            {
                UserId = userId,
                Title = request.Title,
                CategoryId = request.CategoryId,
                Price = request.Price,
                Condition = request.Condition,
                Description = request.Description
            };

            if (request.Images != null)
            {
                foreach (var img in request.Images)
                {
                    var url = await _storage.SaveRequestFileAsync(img);

                    if (url == null) continue;

                    ad.Images.Add(new ProductAdImage
                    {
                        ImageUrl = url
                    });
                }
            }

            await _repo.AddAsync(ad);
            await _repo.SaveAsync();

            return new ApiResponse<int>(true, "Ad created", ad.Id);
        }
    }
}
