using KolsheBjam3etna.BLL.Service.Class;
using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace KolsheBjam3etna.PL.Areas.Identity
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUpdateProfileService _updateProfile;

        public AccountController(IAuthenticationService authenticationService,IUpdateProfileService updateProfile)
        {
            _authenticationService = authenticationService;
            _updateProfile = updateProfile;
        }

       
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var response = await _authenticationService.Register(request);

            if (response == null)
                return BadRequest("Registration failed");

            return Ok(response);
        }

        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authenticationService.Login(request);

            if (response == null)
                return BadRequest("Login failed");

            return Ok(response);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var response = await _authenticationService.ForgotPassword(request.Email);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("verify-reset-code")]
        public async Task<IActionResult> VerifyResetCode([FromBody] VerifyResetCodeRequest request)
        {
            var response = await _authenticationService.VerifyResetCode(request.Email, request.Code);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var response = await _authenticationService.ResetPassword(request.Email, request.Code, request.NewPassword);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpPost("complete-profile")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CompleteProfile([FromForm] CompleteProfileRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new ApiResponse<object>(false, "Unauthorized"));

            var response = await _authenticationService.CompleteProfile(userId, request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        // ---------------- PROFILE ----------------

        [Authorize]
        [HttpPut("profile/personal")]
        public async Task<IActionResult> UpdatePersonal([FromBody] UpdatePersonalProfileRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new ApiResponse<object>(false, "Unauthorized"));

            var response = await _updateProfile.UpdatePersonalProfile(userId, request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpPost("profile/photo")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadPhoto([FromForm] UploadProfilePhotoRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new ApiResponse<object>(false, "Unauthorized"));

            var response = await _updateProfile.UploadProfilePhoto(userId, request.ProfileImage);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpPut("profile/academic")]
        public async Task<IActionResult> UpdateAcademic([FromBody] UpdateAcademicProfileRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new ApiResponse<object>(false, "Unauthorized"));

            var response = await _updateProfile.UpdateAcademicProfile(userId, request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpPut("profile/password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new ApiResponse<object>(false, "Unauthorized"));

            var response = await _updateProfile.ChangePassword(userId, request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new ApiResponse<object>(false, "Unauthorized"));

            var response = await _updateProfile.GetProfile(userId);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}