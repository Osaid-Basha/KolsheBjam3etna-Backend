using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;

namespace KolsheBjam3etna.BLL.Service.Interface
{
    public interface IAdminService
    {
        Task<ApiResponse<AdminDashboardDto>> GetDashboardAsync();
        Task<ApiResponse<List<AdminUserListItemDto>>> GetUsersAsync(string? search, string? status);
        Task<ApiResponse<AdminUserDetailsDto>> GetUserAsync(string userId);
        Task<ApiResponse<string>> UpdateUserAsync(string userId, AdminUpdateUserDto dto);
        Task<ApiResponse<string>> BlockAsync(string userId);
        Task<ApiResponse<string>> UnblockAsync(string userId);
        Task<ApiResponse<string>> DeleteUserAsync(string userId);
    }
}