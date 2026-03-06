using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.BLL.Service.Interface
{
    public interface IPartnerOfferService
    {
        Task<ApiResponse<int>> CreateAsync(CreatePartnerOfferRequest req);
        Task<ApiResponse<string>> UpdateAsync(int id, UpdatePartnerOfferRequest req);
        Task<ApiResponse<List<PartnerOfferListDto>>> GetAllAsync(string? type = null, string? search = null);
        Task<ApiResponse<List<PartnerOfferListDto>>> GetAdminListAsync();
        Task<ApiResponse<PartnerOfferDetailsDto>> GetDetailsAsync(int id);
        Task<ApiResponse<string>> DeleteAsync(int id);
    }
}
