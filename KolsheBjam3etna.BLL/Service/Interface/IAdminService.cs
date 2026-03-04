using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.BLL.Service.Interface
{
    public interface IAdminService
    {
        Task<ApiResponse<AdminDashboardDto>> GetDashboardAsync();
        Task<ApiResponse<List<AdminUserListItemDto>>> GetUsersAsync(string? search, string? status);
        Task<ApiResponse<AdminUserDetailsDto>> GetUserAsync(string userId);

        Task<ApiResponse<string>> BlockAsync(string userId);
        Task<ApiResponse<string>> UnblockAsync(string userId);

    }
}
