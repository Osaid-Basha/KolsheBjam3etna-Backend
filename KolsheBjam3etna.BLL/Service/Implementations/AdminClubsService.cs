using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.Data;
using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace KolsheBjam3etna.BLL.Service.Implementation
{
    public class AdminClubsService : IAdminClubsService
    {
        private readonly ApplicationDbContext _context;

        public AdminClubsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<ClubSummaryDto>> GetSummaryAsync()
        {
            var clubs = await _context.Clubs
                .AsNoTracking()
                .ToListAsync();

            var summary = new ClubSummaryDto
            {
                ActiveCount = clubs.Count(x => GetSubscriptionStatus(x.SubscriptionEndDate) == "Active"),
                ExpiringSoonCount = clubs.Count(x => GetSubscriptionStatus(x.SubscriptionEndDate) == "ExpiringSoon"),
                ExpiredCount = clubs.Count(x => GetSubscriptionStatus(x.SubscriptionEndDate) == "Expired")
            };

            return ApiResponse<ClubSummaryDto>.Ok(summary, "Success");
        }

        public async Task<ApiResponse<List<ClubDto>>> GetAllAsync(string? search = null, string? status = null)
        {
            var clubs = await _context.Clubs
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync();

            var result = clubs.Select(MapToDto).ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim().ToLower();
                result = result.Where(x =>
                    x.Name.ToLower().Contains(keyword) ||
                    x.ManagerName.ToLower().Contains(keyword) ||
                    x.ManagerEmail.ToLower().Contains(keyword) ||
                    x.UniversityName.ToLower().Contains(keyword))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                result = result.Where(x =>
                    x.SubscriptionStatus.Equals(status.Trim(), StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return ApiResponse<List<ClubDto>>.Ok(result, "Success");
        }

        public async Task<ApiResponse<ClubDto>> GetByIdAsync(int id)
        {
            var club = await _context.Clubs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (club == null)
                return ApiResponse<ClubDto>.Fail("Club not found");

            return ApiResponse<ClubDto>.Ok(MapToDto(club), "Success");
        }

        public async Task<ApiResponse<string>> CreateAsync(CreateClubRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Name))
                return ApiResponse<string>.Fail("Club name is required");

            if (string.IsNullOrWhiteSpace(req.UniversityName))
                return ApiResponse<string>.Fail("University name is required");

            if (string.IsNullOrWhiteSpace(req.ManagerName))
                return ApiResponse<string>.Fail("Manager name is required");

            if (string.IsNullOrWhiteSpace(req.ManagerEmail))
                return ApiResponse<string>.Fail("Manager email is required");

            if (string.IsNullOrWhiteSpace(req.SubscriptionType))
                return ApiResponse<string>.Fail("Subscription type is required");

            var normalizedType = NormalizeSubscriptionType(req.SubscriptionType);
            if (normalizedType == null)
                return ApiResponse<string>.Fail("Invalid subscription type");

            var exists = await _context.Clubs.AnyAsync(x => x.ManagerEmail == req.ManagerEmail.Trim());
            if (exists)
                return ApiResponse<string>.Fail("Manager email already used");

            var startDate = DateTime.UtcNow.Date;
            var endDate = normalizedType == "Monthly"
                ? startDate.AddMonths(1)
                : startDate.AddYears(1);

            var club = new Club
            {
                Name = req.Name.Trim(),
                UniversityName = req.UniversityName.Trim(),
                ManagerName = req.ManagerName.Trim(),
                ManagerEmail = req.ManagerEmail.Trim(),
                SubscriptionType = normalizedType,
                SubscriptionPrice = normalizedType == "Monthly" ? 15 : 120,
                SubscriptionStartDate = startDate,
                SubscriptionEndDate = endDate
            };

            _context.Clubs.Add(club);
            await _context.SaveChangesAsync();

            return ApiResponse<string>.Ok("Created", "Club created successfully");
        }

        public async Task<ApiResponse<string>> UpdateAsync(int id, UpdateClubRequest req)
        {
            var club = await _context.Clubs.FirstOrDefaultAsync(x => x.Id == id);
            if (club == null)
                return ApiResponse<string>.Fail("Club not found");

            if (string.IsNullOrWhiteSpace(req.Name))
                return ApiResponse<string>.Fail("Club name is required");

            if (string.IsNullOrWhiteSpace(req.UniversityName))
                return ApiResponse<string>.Fail("University name is required");

            if (string.IsNullOrWhiteSpace(req.ManagerName))
                return ApiResponse<string>.Fail("Manager name is required");

            if (string.IsNullOrWhiteSpace(req.ManagerEmail))
                return ApiResponse<string>.Fail("Manager email is required");

            if (string.IsNullOrWhiteSpace(req.SubscriptionType))
                return ApiResponse<string>.Fail("Subscription type is required");

            var normalizedType = NormalizeSubscriptionType(req.SubscriptionType);
            if (normalizedType == null)
                return ApiResponse<string>.Fail("Invalid subscription type");

            var emailUsedByAnother = await _context.Clubs.AnyAsync(x =>
                x.ManagerEmail == req.ManagerEmail.Trim() && x.Id != id);

            if (emailUsedByAnother)
                return ApiResponse<string>.Fail("Manager email already used by another club");

            club.Name = req.Name.Trim();
            club.UniversityName = req.UniversityName.Trim();
            club.ManagerName = req.ManagerName.Trim();
            club.ManagerEmail = req.ManagerEmail.Trim();

            if (!club.SubscriptionType.Equals(normalizedType, StringComparison.OrdinalIgnoreCase))
            {
                club.SubscriptionType = normalizedType;
                club.SubscriptionPrice = normalizedType == "Monthly" ? 15 : 120;
            }

            await _context.SaveChangesAsync();

            return ApiResponse<string>.Ok("Updated", "Club updated successfully");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var club = await _context.Clubs.FirstOrDefaultAsync(x => x.Id == id);
            if (club == null)
                return ApiResponse<string>.Fail("Club not found");

            _context.Clubs.Remove(club);
            await _context.SaveChangesAsync();

            return ApiResponse<string>.Ok("Deleted", "Club deleted successfully");
        }

        public async Task<ApiResponse<string>> RenewSubscriptionAsync(int id, RenewClubSubscriptionRequest req)
        {
            var club = await _context.Clubs.FirstOrDefaultAsync(x => x.Id == id);
            if (club == null)
                return ApiResponse<string>.Fail("Club not found");

            if (string.IsNullOrWhiteSpace(req.SubscriptionType))
                return ApiResponse<string>.Fail("Subscription type is required");

            var normalizedType = NormalizeSubscriptionType(req.SubscriptionType);
            if (normalizedType == null)
                return ApiResponse<string>.Fail("Invalid subscription type");

            var startDate = DateTime.UtcNow.Date;
            var endDate = normalizedType == "Monthly"
                ? startDate.AddMonths(1)
                : startDate.AddYears(1);

            club.SubscriptionType = normalizedType;
            club.SubscriptionPrice = normalizedType == "Monthly" ? 15 : 120;
            club.SubscriptionStartDate = startDate;
            club.SubscriptionEndDate = endDate;

            await _context.SaveChangesAsync();

            return ApiResponse<string>.Ok("Renewed", "Subscription renewed successfully");
        }

        private static ClubDto MapToDto(Club club)
        {
            return new ClubDto
            {
                Id = club.Id,
                Name = club.Name,
                UniversityName = club.UniversityName,
                ManagerName = club.ManagerName,
                ManagerEmail = club.ManagerEmail,
                SubscriptionType = club.SubscriptionType,
                SubscriptionPrice = club.SubscriptionPrice,
                SubscriptionStartDate = club.SubscriptionStartDate,
                SubscriptionEndDate = club.SubscriptionEndDate,
                SubscriptionStatus = GetSubscriptionStatus(club.SubscriptionEndDate)
            };
        }

        private static string? NormalizeSubscriptionType(string type)
        {
            if (type.Equals("Monthly", StringComparison.OrdinalIgnoreCase))
                return "Monthly";

            if (type.Equals("Yearly", StringComparison.OrdinalIgnoreCase))
                return "Yearly";

            return null;
        }

        private static string GetSubscriptionStatus(DateTime endDate)
        {
            var today = DateTime.UtcNow.Date;

            if (endDate.Date < today)
                return "Expired";

            if (endDate.Date <= today.AddDays(7))
                return "ExpiringSoon";

            return "Active";
        }
    }
}