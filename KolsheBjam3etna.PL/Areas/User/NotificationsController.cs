using KolsheBjam3etna.BLL.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KolsheBjam3etna.PL.Areas.User
{
    [Route("api/notifications")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _service;
        public NotificationsController(INotificationService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetMy([FromQuery] int take = 30)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var res = await _service.GetMyAsync(userId, take);
            return Ok(res);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> UnreadCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var res = await _service.GetUnreadCountAsync(userId);
            return Ok(res);
        }

        [HttpPost("{id:int}/read")]
        public async Task<IActionResult> MarkRead(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var res = await _service.MarkReadAsync(userId, id);
            return res.Success ? Ok(res) : NotFound(res);
        }

        [HttpPost("read-all")]
        public async Task<IActionResult> ReadAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var res = await _service.MarkAllReadAsync(userId);
            return res.Success ? Ok(res) : BadRequest(res);
        }
    }
}
