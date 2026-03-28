using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.Data;
using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace KolsheBjam3etna.BLL.Service.Class
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _dbContext;
        private readonly EmailService _emailService;
        private readonly ILocalFileStorageService _localFileStorage;

        public AuthenticationService(UserManager<ApplicationUser> userManager, IConfiguration config, ApplicationDbContext dbContext, EmailService emailService,ILocalFileStorageService localFileStorage)
        {
            _userManager = userManager;
            _config = config;
            _dbContext = dbContext;
            _emailService = emailService;
            _localFileStorage = localFileStorage;
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return new LoginResponse { Message = "Invalid email or password" };
                }

                var passwordOk = await _userManager.CheckPasswordAsync(user, request.Password);
                if (!passwordOk)
                {
                    return new LoginResponse { Message = "Invalid email or password" };
                }

                var roles = (await _userManager.GetRolesAsync(user)).ToList();
                var token = CreateJwtToken(user, roles);

                return new LoginResponse
                {
                    Message = "Login successful",
                    Token = token,
                    IsProfileCompleted = user.IsProfileCompleted
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse { Message = "An error occurred: " + ex.Message };
            }
        }

        public async Task<RegisterResponse> Register(RegisterRequest request)
        {
            try
            {
                var user = request.Adapt<ApplicationUser>();

          
                if (string.IsNullOrWhiteSpace(user.UserName))
                {
                    
                    user.UserName = request.Email;
                }

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    return new RegisterResponse
                    {
                        Message = "User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description))
                    };
                }

                await _userManager.AddToRoleAsync(user, "User");

                return new RegisterResponse { Message = "User created successfully" };
            }
            catch (Exception ex)
            {
                return new RegisterResponse { Message = "An error occurred: " + ex.Message };
            }
        }

        private string CreateJwtToken(ApplicationUser user, List<string> roles)
        {
            var jwt = _config.GetSection("Jwt");

            var keyBytes = Encoding.UTF8.GetBytes(jwt["Key"]!);
            var key = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Name, user.UserName ?? user.Email ?? "")
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwt["DurationInMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private static string HashCode(string code)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(code));
            return Convert.ToHexString(bytes);
        }

        private static string Generate6DigitCode()
        {
            // 100000 - 999999
            return RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
        }
        public async Task<ApiResponse<object>> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return new ApiResponse<object>(true, "If the email exists, a code was sent.");

            var code = Generate6DigitCode();

            var old = _dbContext.PasswordResetCodes
                .Where(x => x.UserId == user.Id && !x.Used);

            _dbContext.PasswordResetCodes.RemoveRange(old);

            _dbContext.PasswordResetCodes.Add(new PasswordResetCode
            {
                UserId = user.Id,
                CodeHash = HashCode(code),
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(2),
                Used = false
            });

            await _dbContext.SaveChangesAsync();

            var body = $"Your verification code is: {code}";

            await _emailService.SendAsync(
                email,
                "Password Reset Code",
                body
            );

            return new ApiResponse<object>(true, "If the email exists, a code was sent.");
        }
        public async Task<ApiResponse<object>> VerifyResetCode(string email, string code)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new ApiResponse<object>(false, "Invalid or expired code");

            var hash = HashCode(code);

            var rec = await _dbContext.PasswordResetCodes
                .OrderByDescending(x => x.CreatedAtUtc)
                .FirstOrDefaultAsync(x =>
                    x.UserId == user.Id &&
                    !x.Used &&
                    x.CodeHash == hash);

            if (rec == null || rec.ExpiresAtUtc < DateTime.UtcNow)
                return new ApiResponse<object>(false, "Invalid or expired code");

            return new ApiResponse<object>(true, "Code verified successfully");
        }
        public async Task<ApiResponse<object>> ResetPassword(string email, string code, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return new ApiResponse<object>(false, "Invalid code or email");

            var hash = HashCode(code);

            var rec = await _dbContext.PasswordResetCodes
                .OrderByDescending(x => x.CreatedAtUtc)
                .FirstOrDefaultAsync(x =>
                    x.UserId == user.Id &&
                    !x.Used &&
                    x.CodeHash == hash);

            if (rec == null || rec.ExpiresAtUtc < DateTime.UtcNow)
                return new ApiResponse<object>(false, "Invalid or expired code");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
                return new ApiResponse<object>(
                    false,
                    "Reset failed: " + string.Join(", ", result.Errors.Select(e => e.Description))
                );

            rec.Used = true;
            await _dbContext.SaveChangesAsync();

            return new ApiResponse<object>(true, "Password reset successful");
        }
        public async Task<ApiResponse<ProfileResponse>> CompleteProfile(string userId, CompleteProfileRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return new ApiResponse<ProfileResponse>(false, "User not found");

            string? imageUrl = null;

            if (request.ProfileImageUrl != null)
                imageUrl = await _localFileStorage.SaveProfileImageAsync(request.ProfileImageUrl);

            user.UniversityId = request.UniversityId;
            user.Major = request.Major;
            user.Bio = request.Bio;

            if (imageUrl != null)
                user.ProfileImageUrl = imageUrl;

            user.IsProfileCompleted = true;

            await _userManager.UpdateAsync(user);

            string? universityName = null;

            if (user.UniversityId.HasValue)
            {
                universityName = await _dbContext.Universities
                    .Where(u => u.Id == user.UniversityId.Value)
                    .Select(u => u.Name)
                    .FirstOrDefaultAsync();
            }

            var response = new ProfileResponse
            {
                FullName = user.FullName,
                Email = user.Email,
                Bio = user.Bio,
                Major = user.Major,
                UniversityId = request.UniversityId,
                UniversityName = universityName,
                ProfileImageUrl = user.ProfileImageUrl
            };

            return new ApiResponse<ProfileResponse>(
                true,
                "Profile completed successfully",
                response
            );
        }

        public Task<ApiResponse<object>> GetUniversities()
        {
            var universities = _dbContext.Universities
                .Select(u => new { u.Id, u.Name })
                .ToList();
            return Task.FromResult(new ApiResponse<object>(true, "Universities retrieved successfully", universities));

        }
    }

}