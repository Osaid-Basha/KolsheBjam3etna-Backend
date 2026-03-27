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
    public class PartnerOfferService : IPartnerOfferService
    {
        private readonly IPartnerOfferRepository _repo;
        private readonly ILocalFileStorageService _storage;

        public PartnerOfferService(IPartnerOfferRepository repo, ILocalFileStorageService storage)
        {
            _repo = repo;
            _storage = storage;
        }

        public async Task<ApiResponse<int>> CreateAsync(CreatePartnerOfferRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.PartnerName))
                return ApiResponse<int>.Fail("PartnerName is required");

            if (string.IsNullOrWhiteSpace(req.Type))
                return ApiResponse<int>.Fail("Type is required");

            if (string.IsNullOrWhiteSpace(req.Category))
                return ApiResponse<int>.Fail("Category is required");

            if (string.IsNullOrWhiteSpace(req.Title))
                return ApiResponse<int>.Fail("Title is required");

            if (string.IsNullOrWhiteSpace(req.Description))
                return ApiResponse<int>.Fail("Description is required");

            if (req.DiscountPercent < 0 || req.DiscountPercent > 100)
                return ApiResponse<int>.Fail("DiscountPercent must be between 0 and 100");

            if (!Enum.TryParse<PartnerOfferType>(req.Type, true, out var partnerType))
                return ApiResponse<int>.Fail("Invalid Type");

            string? imageUrl = null;
            if (req.Image != null)
                imageUrl = await _storage.SavePartnerOfferImageAsync(req.Image);

            var offer = new PartnerOffer
            {
                PartnerName = req.PartnerName.Trim(),
                Type = partnerType,
                Category = req.Category.Trim(),
                Title = req.Title.Trim(),
                Description = req.Description.Trim(),
                Location = req.Location.Trim(),
                Phone = req.Phone.Trim(),
                Email = req.Email.Trim(),
                ImageUrl = imageUrl,
                DiscountPercent = req.DiscountPercent,
                IsVerified = true,
                ShowOnHomePage = req.ShowOnHomePage,
                Rating = 4.8,
                RatingsCount = 124,
                ExpireDateUtc = req.ExpireDateUtc
            };

            await _repo.AddAsync(offer);
            await _repo.SaveAsync();

            return ApiResponse<int>.Ok(offer.Id, "Offer created");
        }

        public async Task<ApiResponse<string>> UpdateAsync(int id, UpdatePartnerOfferRequest req)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return ApiResponse<string>.Fail("Offer not found");

            if (!Enum.TryParse<PartnerOfferType>(req.Type, true, out var parsedType))
                return ApiResponse<string>.Fail("Invalid Type");

            string? imageUrl = null;
            if (req.Image != null)
                imageUrl = await _storage.SavePartnerOfferImageAsync(req.Image);

            entity.PartnerName = req.PartnerName.Trim();
            entity.Type = parsedType;
            entity.Category = req.Category.Trim();
            entity.Title = req.Title.Trim();
            entity.Description = req.Description.Trim();
            entity.Location = req.Location.Trim();
            entity.Phone = req.Phone.Trim();
            entity.Email = req.Email.Trim();
            entity.ExpireDateUtc = req.ExpireDateUtc;
            entity.DiscountPercent = req.DiscountPercent;
            entity.ShowOnHomePage = req.ShowOnHomePage;
            entity.IsVerified = req.IsVerified;

            if (!string.IsNullOrEmpty(imageUrl))
                entity.ImageUrl = imageUrl;

            await _repo.SaveAsync();

            return ApiResponse<string>.Ok("Updated", "Offer updated");
        }

        public async Task<ApiResponse<List<PartnerOfferListDto>>> GetAllAsync(string? type = null, string? search = null)
        {
            var items = await _repo.GetAllAsync(type, search);
            return ApiResponse<List<PartnerOfferListDto>>.Ok(items, "Success");
        }

        public async Task<ApiResponse<List<PartnerOfferListDto>>> GetAdminListAsync()
        {
            var items = await _repo.GetAdminListAsync();
            return ApiResponse<List<PartnerOfferListDto>>.Ok(items, "Success");
        }

        public async Task<ApiResponse<PartnerOfferDetailsDto>> GetDetailsAsync(int id)
        {
            var dto = await _repo.GetDetailsAsync(id);
            if (dto == null) return ApiResponse<PartnerOfferDetailsDto>.Fail("Offer not found");

            return ApiResponse<PartnerOfferDetailsDto>.Ok(dto, "Success");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return ApiResponse<string>.Fail("Offer not found");

            _repo.Remove(entity);
            await _repo.SaveAsync();

            return ApiResponse<string>.Ok("Deleted", "Offer deleted");
        }
    }
}
