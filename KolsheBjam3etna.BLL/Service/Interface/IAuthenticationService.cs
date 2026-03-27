using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;

namespace KolsheBjam3etna.BLL.Service.Interface
{
    public interface IAuthenticationService
    {
        Task<RegisterResponse> Register(RegisterRequest request);
        Task<LoginResponse> Login(LoginRequest request);

        Task<ApiResponse<object>> ForgotPassword(string email);

        Task<ApiResponse<object>> VerifyResetCode(string email, string code);

        Task<ApiResponse<object>> ResetPassword(string email, string code, string newPassword);

        Task<ApiResponse<ProfileResponse>> CompleteProfile(string userId, CompleteProfileRequest request);
        
        Task<ApiResponse<object>> GetUniversities();
    }
}