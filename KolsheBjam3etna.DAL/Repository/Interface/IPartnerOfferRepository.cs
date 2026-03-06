using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Repository.Interface
{
    public interface IPartnerOfferRepository
    {
        Task AddAsync(PartnerOffer offer);
        Task SaveAsync();

        Task<PartnerOffer?> GetByIdAsync(int id);

        Task<List<PartnerOfferListDto>> GetAllAsync(string? type = null, string? search = null);
        Task<List<PartnerOfferListDto>> GetAdminListAsync();
        Task<PartnerOfferDetailsDto?> GetDetailsAsync(int id);

        void Remove(PartnerOffer offer);
    }
}
