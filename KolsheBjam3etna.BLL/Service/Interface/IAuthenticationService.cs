using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;

namespace KolsheBjam3etna.BLL.Service.Interface
{
    public interface IAuthenticationService
    {
        Task<RegisterResponse> Register(RegisterRequest request);
        Task<LoginResponse> Login(LoginRequest request);

        Task<string> ForgotPassword(string email);
        Task<bool> VerifyResetCode(string email, string code);
        Task<string> ResetPassword(string email, string code, string newPassword);
        Task<string> CompleteProfile(string userId, CompleteProfileRequest request);
    }
}