using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace KolsheBjam3etna.BLL.Service.Class
{
    public class UpdateProfileService : IUpdateProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILocalFileStorageService _localFileStorage;

        public UpdateProfileService(
            UserManager<ApplicationUser> userManager,
            ILocalFileStorageService localFileStorage)
        {
            _userManager = userManager;
            _localFileStorage = localFileStorage;
        }

        public async Task<ApiResponse<ProfileResponse>> UpdatePersonalProfile(string userId, UpdatePersonalProfileRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ApiResponse<ProfileResponse>(false, "User not found");

            if (!string.IsNullOrWhiteSpace(request.FullName)) user.FullName = request.FullName;
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber)) user.PhoneNumber = request.PhoneNumber;
            if (request.Bio != null) user.Bio = request.Bio;
            if (request.WebsiteUrl != null) user.WebsiteUrl = request.WebsiteUrl;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return new ApiResponse<ProfileResponse>(false, "Update failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));

            return new ApiResponse<ProfileResponse>(true, "Personal profile updated", MapToProfileResponse(user));
        }

        public async Task<ApiResponse<ProfileResponse>> UpdateAcademicProfile(string userId, UpdateAcademicProfileRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ApiResponse<ProfileResponse>(false, "User not found");

            user.UniversityId = request.UniversityId;
            user.Major = request.Major;
            user.StudyYear = request.StudyYear;
            user.UniversityNumber = request.UniversityNumber;

          
            user.IsProfileCompleted = true;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return new ApiResponse<ProfileResponse>(false, "Update failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));

            return new ApiResponse<ProfileResponse>(true, "Academic profile updated", MapToProfileResponse(user));
        }

        public async Task<ApiResponse<object>> ChangePassword(string userId, ChangePasswordRequest request)
        {
            if (request.NewPassword != request.ConfirmNewPassword)
                return new ApiResponse<object>(false, "Passwords do not match");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ApiResponse<object>(false, "User not found");

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
                return new ApiResponse<object>(false, "Change failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));

            return new ApiResponse<object>(true, "Password updated");
        }

        public async Task<ApiResponse<UploadProfilePhotoResponse>> UploadProfilePhoto(string userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return new ApiResponse<UploadProfilePhotoResponse>(false, "File is required");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ApiResponse<UploadProfilePhotoResponse>(false, "User not found");

            var url = await _localFileStorage.SaveProfileImageAsync(file);
            if (string.IsNullOrWhiteSpace(url))
                return new ApiResponse<UploadProfilePhotoResponse>(false, "Upload failed");

            user.ProfileImageUrl = url;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return new ApiResponse<UploadProfilePhotoResponse>(false, "Update failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));

            return new ApiResponse<UploadProfilePhotoResponse>(
                true,
                "Profile photo uploaded successfully",
                new UploadProfilePhotoResponse { ProfileImageUrl = url }
            );
        }

        public async Task<ApiResponse<ProfileResponse>> GetProfile(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ApiResponse<ProfileResponse>(false, "User not found");

            return new ApiResponse<ProfileResponse>(true, "Profile loaded", MapToProfileResponse(user));
        }

        private static ProfileResponse MapToProfileResponse(ApplicationUser user)
        {
            return new ProfileResponse
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Bio = user.Bio,
                WebsiteUrl = user.WebsiteUrl,
                ProfileImageUrl = user.ProfileImageUrl,
                UniversityId = user.UniversityId,
                Major = user.Major,
                StudyYear = user.StudyYear,
                UniversityNumber = user.UniversityNumber,
            };
        }
    }
}