using KolsheBjam3etna.BLL.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KolsheBjam3etna.PL.Areas.Coordinator
{
    [Route("api/Coordinator")]
    [ApiController]
    [Authorize(Roles = "Coordinator")]
    public class CoordinatorController : ControllerBase
    {
        private readonly IEventService _service;
        public CoordinatorController(IEventService service) => _service = service;

        [HttpGet("events")]
        public async Task<IActionResult> MyEvents()
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var res = await _service.GetMyEventsAsync(userId);
            return Ok(res);
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var res = await _service.GetDashboardAsync(userId);
            return Ok(res);
        }
    }
}
