using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class AdminRolesService : IAdminRolesService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    private static readonly HashSet<string> AllowedRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "Admin",
        "SuperAdmin",
        "NewsEditor",
        "Coordinator"
    };

    public AdminRolesService(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<ApiResponse<List<AdminRoleSummaryDto>>> GetRolesSummaryAsync()
    {
        
        var roles = await _roleManager.Roles.AsNoTracking().ToListAsync();

        var result = new List<AdminRoleSummaryDto>();

        foreach (var r in roles)
        {
           
            var users = await _userManager.GetUsersInRoleAsync(r.Name!);
            result.Add(new AdminRoleSummaryDto
            {
                Role = r.Name!,
                UsersCount = users.Count
            });
        }

     
        result = result
            .OrderBy(x => x.Role == "SuperAdmin" ? 0 :
                          x.Role == "Admin" ? 1 :
                          x.Role == "NewsEditor" ? 2 :
                          x.Role == "Coordinator" ? 3 : 99)
            .ToList();

        return ApiResponse<List<AdminRoleSummaryDto>>.Ok(result, "Success");
    }

    public async Task<ApiResponse<List<string>>> GetUserRolesAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return ApiResponse<List<string>>.Fail("Email is required");

        var user = await _userManager.FindByEmailAsync(email.Trim());
        if (user == null)
            return ApiResponse<List<string>>.Fail("User not found");

        var roles = await _userManager.GetRolesAsync(user);
        return ApiResponse<List<string>>.Ok(roles.ToList(), "Success");
    }

    public async Task<ApiResponse<string>> AssignRoleAsync(AssignRoleByEmailRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Email))
            return ApiResponse<string>.Fail("Email is required");

        if (string.IsNullOrWhiteSpace(req.Role))
            return ApiResponse<string>.Fail("Role is required");

        var role = req.Role.Trim();
        if (!AllowedRoles.Contains(role))
            return ApiResponse<string>.Fail("Invalid role");

        var user = await _userManager.FindByEmailAsync(req.Email.Trim());
        if (user == null)
            return ApiResponse<string>.Fail("User not found");

       
        var roleExists = await _roleManager.RoleExistsAsync(role);
        if (!roleExists)
        {
            var created = await _roleManager.CreateAsync(new IdentityRole(role));
            if (!created.Succeeded)
                return ApiResponse<string>.Fail("Failed to create role");
        }

       
        if (await _userManager.IsInRoleAsync(user, role))
            return ApiResponse<string>.Fail("User already has this role");

        
        if (role.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase))
        {
            if (await _userManager.IsInRoleAsync(user, "Admin"))
                await _userManager.RemoveFromRoleAsync(user, "Admin");
        }

        var res = await _userManager.AddToRoleAsync(user, role);
        if (!res.Succeeded)
            return ApiResponse<string>.Fail("Failed to assign role");

        return ApiResponse<string>.Ok("Assigned", "Role assigned");
    }

    public async Task<ApiResponse<string>> RemoveRoleAsync(RemoveRoleByEmailRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Email))
            return ApiResponse<string>.Fail("Email is required");

        if (string.IsNullOrWhiteSpace(req.Role))
            return ApiResponse<string>.Fail("Role is required");

        var role = req.Role.Trim();
        if (!AllowedRoles.Contains(role))
            return ApiResponse<string>.Fail("Invalid role");

        var user = await _userManager.FindByEmailAsync(req.Email.Trim());
        if (user == null)
            return ApiResponse<string>.Fail("User not found");

        if (!await _userManager.IsInRoleAsync(user, role))
            return ApiResponse<string>.Fail("User doesn't have this role");

        var res = await _userManager.RemoveFromRoleAsync(user, role);
        if (!res.Succeeded)
            return ApiResponse<string>.Fail("Failed to remove role");

        return ApiResponse<string>.Ok("Removed", "Role removed");
    }
}