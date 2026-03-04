using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.DTOs.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KolsheBjam3etna.PL.Areas.User
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SwapAdsController : ControllerBase
    {
        private readonly ISwapAdService _service;

        public SwapAdsController(ISwapAdService service)
        {
            _service = service;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateSwapAdRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var result = await _service.CreateAsync(userId, request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
