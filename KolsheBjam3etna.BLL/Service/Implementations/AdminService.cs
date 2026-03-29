using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.Data;
using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KolsheBjam3etna.BLL.Service.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminService(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<ApiResponse<AdminDashboardDto>> GetDashboardAsync()
        {
            var usersCount = await _db.Users.CountAsync();
            var messagesCount = await _db.Messages.CountAsync();

            var adsCount =
                await _db.ProductAds.CountAsync() +
                await _db.SwapAds.CountAsync() +
                await _db.ServiceRequests.CountAsync();

            var dto = new AdminDashboardDto
            {
                AdsCount = adsCount,
                UsersCount = usersCount,
                MessagesCount = messagesCount,
                ReportsCount = 0
            };

            return ApiResponse<AdminDashboardDto>.Ok(dto, "Success");
        }

        public async Task<ApiResponse<List<AdminUserListItemDto>>> GetUsersAsync(string? search, string? status)
        {
            var now = DateTimeOffset.UtcNow;

            var q = _db.Users
                .AsNoTracking()
                .Include(u => u.University)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                q = q.Where(u =>
                    u.FullName.Contains(search) ||
                    (u.Email != null && u.Email.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.Trim().ToLowerInvariant();

                if (status == "blocked" || status == "inactive" || status == "موقوف")
                    q = q.Where(u => u.LockoutEnd != null && u.LockoutEnd > now);
                else if (status == "active" || status == "نشط")
                    q = q.Where(u => u.LockoutEnd == null || u.LockoutEnd <= now);
            }

            var list = await q
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new AdminUserListItemDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email ?? "",
                    ProfileImageUrl = u.ProfileImageUrl,
                    UniversityName = u.University != null ? u.University.Name : null,
                    PostsCount =
                        _db.ProductAds.Count(pa => pa.UserId == u.Id) +
                        _db.SwapAds.Count(sa => sa.UserId == u.Id) +
                        _db.ServiceRequests.Count(sr => sr.UserId == u.Id),
                    IsBlocked = u.LockoutEnd != null && u.LockoutEnd > now
                })
                .ToListAsync();

            return ApiResponse<List<AdminUserListItemDto>>.Ok(list, "Success");
        }

        public async Task<ApiResponse<AdminUserDetailsDto>> GetUserAsync(string userId)
        {
            var now = DateTimeOffset.UtcNow;

            var user = await _db.Users
                .Include(u => u.University)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return ApiResponse<AdminUserDetailsDto>.Fail("User not found");

            var roles = await _userManager.GetRolesAsync(user);

            var dto = new AdminUserDetailsDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "",
                ProfileImageUrl = user.ProfileImageUrl,
                Major = user.Major,
                StudyYear = user.StudyYear,
                UniversityNumber = user.UniversityNumber,
                Bio = user.Bio,
                WebsiteUrl = user.WebsiteUrl,
                UniversityName = user.University?.Name,
                IsBlocked = user.LockoutEnd != null && user.LockoutEnd > now,
                Roles = roles.ToList(),
                ProductAdsCount = await _db.ProductAds.CountAsync(x => x.UserId == user.Id),
                SwapAdsCount = await _db.SwapAds.CountAsync(x => x.UserId == user.Id),
                ServiceRequestsCount = await _db.ServiceRequests.CountAsync(x => x.UserId == user.Id)
            };

            return ApiResponse<AdminUserDetailsDto>.Ok(dto, "Success");
        }

        public async Task<ApiResponse<string>> UpdateUserAsync(string userId, AdminUpdateUserDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return ApiResponse<string>.Fail("User not found");

            var emailExists = await _db.Users.AnyAsync(x => x.Id != userId && x.Email == dto.Email);
            if (emailExists)
                return ApiResponse<string>.Fail("Email already exists");

            user.FullName = dto.FullName.Trim();
            user.Email = dto.Email.Trim();
            user.UserName = dto.Email.Trim();
            user.NormalizedEmail = dto.Email.Trim().ToUpper();
            user.NormalizedUserName = dto.Email.Trim().ToUpper();
            user.UniversityId = dto.UniversityId;

            if (dto.IsBlocked)
            {
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.MaxValue;
            }
            else
            {
                user.LockoutEnd = null;
            }

            await _db.SaveChangesAsync();
            return ApiResponse<string>.Ok("Updated", "User updated successfully");
        }

        public async Task<ApiResponse<string>> BlockAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponse<string>.Fail("User not found");

            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;

            var res = await _userManager.UpdateAsync(user);
            if (!res.Succeeded)
                return ApiResponse<string>.Fail("Failed to block user");

            return ApiResponse<string>.Ok("Blocked", "User blocked");
        }

        public async Task<ApiResponse<string>> UnblockAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponse<string>.Fail("User not found");

            user.LockoutEnd = null;

            var res = await _userManager.UpdateAsync(user);
            if (!res.Succeeded)
                return ApiResponse<string>.Fail("Failed to unblock user");

            return ApiResponse<string>.Ok("Unblocked", "User unblocked");
        }

        public async Task<ApiResponse<string>> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponse<string>.Fail("User not found");

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("SuperAdmin"))
                return ApiResponse<string>.Fail("You cannot delete SuperAdmin");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return ApiResponse<string>.Fail("Failed to delete user");

            return ApiResponse<string>.Ok("Deleted", "User deleted successfully");
        }
    }
}