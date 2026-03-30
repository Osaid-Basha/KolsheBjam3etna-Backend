using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;

namespace KolsheBjam3etna.BLL.Service.Interface
{
    public interface IAdminClubsService
    {
        Task<ApiResponse<ClubSummaryDto>> GetSummaryAsync();
        Task<ApiResponse<List<ClubDto>>> GetAllAsync(string? search = null, string? status = null);
        Task<ApiResponse<ClubDto>> GetByIdAsync(int id);
        Task<ApiResponse<string>> CreateAsync(CreateClubRequest req);
        Task<ApiResponse<string>> UpdateAsync(int id, UpdateClubRequest req);
        Task<ApiResponse<string>> DeleteAsync(int id);
        Task<ApiResponse<string>> RenewSubscriptionAsync(int id, RenewClubSubscriptionRequest req);
    }
}