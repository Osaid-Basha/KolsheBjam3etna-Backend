using KolsheBjam3etna.DAL.Data;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using KolsheBjam3etna.DAL.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace KolsheBjam3etna.DAL.Repository.Implementations
{
    public class PartnerOfferRepository : IPartnerOfferRepository
    {
        private readonly ApplicationDbContext _db;

        public PartnerOfferRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(PartnerOffer offer) => await _db.PartnerOffers.AddAsync(offer);

        public async Task SaveAsync() => await _db.SaveChangesAsync();

        public async Task<PartnerOffer?> GetByIdAsync(int id)
            => await _db.PartnerOffers.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<List<PartnerOfferListDto>> GetAllAsync(string? type = null, string? search = null)
        {
           var q = _db.PartnerOffers
    .AsNoTracking()
    .AsQueryable();

            if (!string.IsNullOrWhiteSpace(type) &&
                Enum.TryParse<PartnerOfferType>(type, true, out var parsedType))
            {
                q = q.Where(x => x.Type == parsedType);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                q = q.Where(x =>
                    x.PartnerName.Contains(search) ||
                    x.Title.Contains(search) ||
                    x.Category.Contains(search));
            }

            return await q
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new PartnerOfferListDto
                {
                    Id = x.Id,
                    PartnerName = x.PartnerName,
                    Type = x.Type.ToString(),
                    Category = x.Category,
                    Title = x.Title,
                    Description = x.Description,
                    Location = x.Location,
                    DiscountPercent = x.DiscountPercent,
                    Rating = x.Rating,
                    RatingsCount = x.RatingsCount,
                    ExpireDateUtc = x.ExpireDateUtc,
                    ImageUrl = x.ImageUrl,
                    IsVerified = x.IsVerified,
                    ShowOnHomePage = x.ShowOnHomePage
                })
                .ToListAsync();
        }

        public async Task<List<PartnerOfferListDto>> GetAdminListAsync()
        {
            return await _db.PartnerOffers
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new PartnerOfferListDto
                {
                    Id = x.Id,
                    PartnerName = x.PartnerName,
                    Type = x.Type.ToString(),
                    Category = x.Category,
                    Title = x.Title,
                    Description = x.Description,
                    Location = x.Location,
                    DiscountPercent = x.DiscountPercent,
                    Rating = x.Rating,
                    RatingsCount = x.RatingsCount,
                    ExpireDateUtc = x.ExpireDateUtc,
                    ImageUrl = x.ImageUrl,
                    IsVerified = x.IsVerified,
                    ShowOnHomePage = x.ShowOnHomePage
                })
                .ToListAsync();
        }

        public async Task<PartnerOfferDetailsDto?> GetDetailsAsync(int id)
        {
            return await _db.PartnerOffers
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new PartnerOfferDetailsDto
                {
                    Id = x.Id,
                    PartnerName = x.PartnerName,
                    Type = x.Type.ToString(),
                    Category = x.Category,
                    Title = x.Title,
                    Description = x.Description,
                    Location = x.Location,
                    Phone = x.Phone,
                    Email = x.Email,
                    DiscountPercent = x.DiscountPercent,
                    ExpireDateUtc = x.ExpireDateUtc,
                    ImageUrl = x.ImageUrl,
                    IsVerified = x.IsVerified,
                    ShowOnHomePage = x.ShowOnHomePage,
                    Rating = x.Rating,
                    RatingsCount = x.RatingsCount
                })
                .FirstOrDefaultAsync();
        }

        public void Remove(PartnerOffer offer) => _db.PartnerOffers.Remove(offer);
    }
}
