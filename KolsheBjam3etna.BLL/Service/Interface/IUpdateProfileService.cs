using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using Microsoft.AspNetCore.Http;

namespace KolsheBjam3etna.BLL.Service.Interface
{
    public interface IUpdateProfileService
    {
        Task<ApiResponse<ProfileResponse>> UpdatePersonalProfile(string userId, UpdatePersonalProfileRequest request);

        Task<ApiResponse<ProfileResponse>> UpdateAcademicProfile(string userId, UpdateAcademicProfileRequest request);

        Task<ApiResponse<object>> ChangePassword(string userId, ChangePasswordRequest request);

        Task<ApiResponse<ProfileResponse>> GetProfile(string userId);

        Task<ApiResponse<UploadProfilePhotoResponse>> UploadProfilePhoto(string userId, IFormFile file);
    }
}