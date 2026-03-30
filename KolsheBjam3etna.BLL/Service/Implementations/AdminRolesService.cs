using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.Data;
using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class AdminRolesService : IAdminRolesService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    private static readonly List<string> AllowedRoles = new()
    {
        "SuperAdmin",
        "Admin",
        "NewsEditor",
        "Coordinator"
    };

    private static readonly Dictionary<string, List<string>> RolePermissions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["SuperAdmin"] = new()
        {
            "إدارة الأدوار",
            "إدارة المستخدمين",
            "نشر الأخبار",
            "تعديل الأخبار",
            "إحصاءات عامة"
        },
        ["Admin"] = new()
        {
            "إدارة المستخدمين",
            "عرض الأخبار"
        },
        ["NewsEditor"] = new()
        {
            "نشر الأخبار",
            "تعديل الأخبار"
        },
        ["Coordinator"] = new()
        {
            "إدارة النادي",
            "إنشاء الفعاليات",
            "تعديل الفعاليات",
            "عرض المسجلين"
        }
    };

    public AdminRolesService(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _context = context;
    }

    public async Task<ApiResponse<List<AdminRoleSummaryDto>>> GetRolesSummaryAsync()
    {
        var result = new List<AdminRoleSummaryDto>();

        foreach (var role in AllowedRoles)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);
            result.Add(new AdminRoleSummaryDto
            {
                Role = role,
                UsersCount = users.Count
            });
        }

        result = result
            .OrderBy(x => GetRoleOrder(x.Role))
            .ToList();

        return ApiResponse<List<AdminRoleSummaryDto>>.Ok(result, "Success");
    }

    public async Task<ApiResponse<List<AdminRoleUserDto>>> GetAllRoleUsersAsync(string? search = null, string? role = null)
    {
        var users = await _context.Users
            .AsNoTracking()
            .ToListAsync();

        var clubs = await _context.Clubs
            .AsNoTracking()
            .ToListAsync();

        var result = new List<AdminRoleUserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var adminRole = roles.FirstOrDefault(r => AllowedRoles.Contains(r));

            if (string.IsNullOrWhiteSpace(adminRole))
                continue;

            var relatedClub = clubs.FirstOrDefault(c => c.OwnerId == user.Id);

            result.Add(new AdminRoleUserDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Role = adminRole,
                ClubId = relatedClub?.Id,
                ClubName = relatedClub?.Name,
                CreatedAt = user.CreatedAt,
                Permissions = GetPermissions(adminRole)
            });
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim().ToLower();
            result = result.Where(x =>
                x.FullName.ToLower().Contains(keyword) ||
                x.Email.ToLower().Contains(keyword) ||
                (x.ClubName != null && x.ClubName.ToLower().Contains(keyword)))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(role))
        {
            result = result
                .Where(x => x.Role.Equals(role.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        result = result
            .OrderBy(x => GetRoleOrder(x.Role))
            .ThenBy(x => x.FullName)
            .ToList();

        return ApiResponse<List<AdminRoleUserDto>>.Ok(result, "Success");
    }

    public async Task<ApiResponse<AdminRoleUserDto>> GetUserRoleAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return ApiResponse<AdminRoleUserDto>.Fail("Email is required");

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email.Trim());

        if (user == null)
            return ApiResponse<AdminRoleUserDto>.Fail("User not found");

        var roles = await _userManager.GetRolesAsync(user);
        var adminRole = roles.FirstOrDefault(r => AllowedRoles.Contains(r));

        if (string.IsNullOrWhiteSpace(adminRole))
            return ApiResponse<AdminRoleUserDto>.Fail("User has no admin role");

        var relatedClub = await _context.Clubs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.OwnerId == user.Id);

        var dto = new AdminRoleUserDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Role = adminRole,
            ClubId = relatedClub?.Id,
            ClubName = relatedClub?.Name,
            CreatedAt = user.CreatedAt,
            Permissions = GetPermissions(adminRole)
        };

        return ApiResponse<AdminRoleUserDto>.Ok(dto, "Success");
    }

    public async Task<ApiResponse<List<RoleOptionDto>>> GetAvailableRolesAsync()
    {
        var result = AllowedRoles
            .Select(x => new RoleOptionDto
            {
                Role = x,
                Permissions = GetPermissions(x)
            })
            .OrderBy(x => GetRoleOrder(x.Role))
            .ToList();

        return ApiResponse<List<RoleOptionDto>>.Ok(result, "Success");
    }

    public async Task<ApiResponse<List<ClubOptionDto>>> GetClubOptionsAsync()
    {
        var clubs = await _context.Clubs
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new ClubOptionDto
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();

        return ApiResponse<List<ClubOptionDto>>.Ok(clubs, "Success");
    }

    public async Task<ApiResponse<string>> AssignRoleAsync(AssignRoleByEmailRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Email))
            return ApiResponse<string>.Fail("Email is required");

        if (string.IsNullOrWhiteSpace(req.Role))
            return ApiResponse<string>.Fail("Role is required");

        var normalizedRole = NormalizeRole(req.Role);
        if (normalizedRole == null)
            return ApiResponse<string>.Fail("Invalid role");

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == req.Email.Trim());
        if (user == null)
            return ApiResponse<string>.Fail("User not found");

        foreach (var role in AllowedRoles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                var createRoleResult = await _roleManager.CreateAsync(new IdentityRole(role));
                if (!createRoleResult.Succeeded)
                    return ApiResponse<string>.Fail($"Failed to create role: {role}");
            }
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        var currentAdminRoles = currentRoles.Where(r => AllowedRoles.Contains(r)).ToList();

        if (currentAdminRoles.Any())
            return ApiResponse<string>.Fail("User already has an admin role, use update instead");

        if (normalizedRole == "Coordinator")
        {
            if (!req.ClubId.HasValue)
                return ApiResponse<string>.Fail("ClubId is required for Coordinator");

            var club = await _context.Clubs.FirstOrDefaultAsync(x => x.Id == req.ClubId.Value);
            if (club == null)
                return ApiResponse<string>.Fail("Selected club not found");

            if (!string.IsNullOrWhiteSpace(club.OwnerId) && club.OwnerId != user.Id)
                return ApiResponse<string>.Fail("This club already has an owner");

            var anotherOwnedClub = await _context.Clubs.AnyAsync(x => x.OwnerId == user.Id);
            if (anotherOwnedClub)
                return ApiResponse<string>.Fail("This user already owns another club");

            club.OwnerId = user.Id;
        }

        var addResult = await _userManager.AddToRoleAsync(user, normalizedRole);
        if (!addResult.Succeeded)
            return ApiResponse<string>.Fail("Failed to assign role");

        await _context.SaveChangesAsync();

        return ApiResponse<string>.Ok("Assigned", "Role assigned successfully");
    }

    public async Task<ApiResponse<string>> UpdateRoleAsync(UpdateRoleByEmailRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Email))
            return ApiResponse<string>.Fail("Email is required");

        if (string.IsNullOrWhiteSpace(req.NewRole))
            return ApiResponse<string>.Fail("NewRole is required");

        var normalizedRole = NormalizeRole(req.NewRole);
        if (normalizedRole == null)
            return ApiResponse<string>.Fail("Invalid role");

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == req.Email.Trim());
        if (user == null)
            return ApiResponse<string>.Fail("User not found");

        var currentRoles = await _userManager.GetRolesAsync(user);
        var currentAdminRoles = currentRoles.Where(r => AllowedRoles.Contains(r)).ToList();

        foreach (var oldRole in currentAdminRoles)
        {
            var removeOldRole = await _userManager.RemoveFromRoleAsync(user, oldRole);
            if (!removeOldRole.Succeeded)
                return ApiResponse<string>.Fail("Failed to remove old role");
        }

        var currentOwnedClubs = await _context.Clubs
            .Where(x => x.OwnerId == user.Id)
            .ToListAsync();

        foreach (var club in currentOwnedClubs)
        {
            club.OwnerId = null;
        }

        if (normalizedRole == "Coordinator")
        {
            if (!req.ClubId.HasValue)
                return ApiResponse<string>.Fail("ClubId is required for Coordinator");

            var selectedClub = await _context.Clubs.FirstOrDefaultAsync(x => x.Id == req.ClubId.Value);
            if (selectedClub == null)
                return ApiResponse<string>.Fail("Selected club not found");

            if (!string.IsNullOrWhiteSpace(selectedClub.OwnerId) && selectedClub.OwnerId != user.Id)
                return ApiResponse<string>.Fail("This club already has an owner");

            selectedClub.OwnerId = user.Id;
        }

        var addNewRole = await _userManager.AddToRoleAsync(user, normalizedRole);
        if (!addNewRole.Succeeded)
            return ApiResponse<string>.Fail("Failed to assign new role");

        await _context.SaveChangesAsync();

        return ApiResponse<string>.Ok("Updated", "Role updated successfully");
    }

    public async Task<ApiResponse<string>> RemoveRoleAsync(RemoveRoleByEmailRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Email))
            return ApiResponse<string>.Fail("Email is required");

        if (string.IsNullOrWhiteSpace(req.Role))
            return ApiResponse<string>.Fail("Role is required");

        var normalizedRole = NormalizeRole(req.Role);
        if (normalizedRole == null)
            return ApiResponse<string>.Fail("Invalid role");

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == req.Email.Trim());
        if (user == null)
            return ApiResponse<string>.Fail("User not found");

        if (!await _userManager.IsInRoleAsync(user, normalizedRole))
            return ApiResponse<string>.Fail("User doesn't have this role");

        var removeResult = await _userManager.RemoveFromRoleAsync(user, normalizedRole);
        if (!removeResult.Succeeded)
            return ApiResponse<string>.Fail("Failed to remove role");

        var ownedClubs = await _context.Clubs
            .Where(x => x.OwnerId == user.Id)
            .ToListAsync();

        foreach (var club in ownedClubs)
        {
            club.OwnerId = null;
        }

        await _context.SaveChangesAsync();

        return ApiResponse<string>.Ok("Removed", "Role removed successfully");
    }

    private static string? NormalizeRole(string role)
    {
        return AllowedRoles.FirstOrDefault(x => x.Equals(role.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    private static List<string> GetPermissions(string role)
    {
        return RolePermissions.TryGetValue(role, out var permissions)
            ? permissions
            : new List<string>();
    }

    private static int GetRoleOrder(string role)
    {
        return role switch
        {
            "SuperAdmin" => 0,
            "Admin" => 1,
            "NewsEditor" => 2,
            "Coordinator" => 3,
            _ => 99
        };
    }
}