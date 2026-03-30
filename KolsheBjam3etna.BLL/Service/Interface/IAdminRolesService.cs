using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;

namespace KolsheBjam3etna.BLL.Service.Interface
{
    public interface IAdminRolesService
    {
        Task<ApiResponse<List<AdminRoleSummaryDto>>> GetRolesSummaryAsync();
        Task<ApiResponse<List<AdminRoleUserDto>>> GetAllRoleUsersAsync(string? search = null, string? role = null);
        Task<ApiResponse<AdminRoleUserDto>> GetUserRoleAsync(string email);
        Task<ApiResponse<List<RoleOptionDto>>> GetAvailableRolesAsync();
        Task<ApiResponse<List<ClubOptionDto>>> GetClubOptionsAsync();
        Task<ApiResponse<string>> AssignRoleAsync(AssignRoleByEmailRequest req);
        Task<ApiResponse<string>> UpdateRoleAsync(UpdateRoleByEmailRequest req);
        Task<ApiResponse<string>> RemoveRoleAsync(RemoveRoleByEmailRequest req);
    }
}