using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.BLL.Service.Interface
{
    public interface IAdminRolesService
    {
        Task<ApiResponse<List<AdminRoleSummaryDto>>> GetRolesSummaryAsync();
        Task<ApiResponse<List<string>>> GetUserRolesAsync(string email);

        Task<ApiResponse<string>> AssignRoleAsync(AssignRoleByEmailRequest req);
        Task<ApiResponse<string>> RemoveRoleAsync(RemoveRoleByEmailRequest req);
    }
}
