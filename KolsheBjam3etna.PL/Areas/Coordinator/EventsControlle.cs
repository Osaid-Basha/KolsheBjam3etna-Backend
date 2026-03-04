using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.DTOs.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KolsheBjam3etna.PL.Areas.Coordinator
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Coordinator")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _service;
        public EventsController(IEventService service) => _service = service;

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateEventRequest req)
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var res = await _service.CreateAsync(userId, req);
            return res.Success ? Ok(res) : BadRequest(res);
        }
        [HttpGet("events/{eventId:int}/registrations")]
        public async Task<IActionResult> Registrations(int eventId)
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var res = await _service.GetRegistrationsAsync(userId, eventId);
            return Ok(res);
        }
        [HttpDelete("events/{eventId:int}")]
        public async Task<IActionResult> Delete(int eventId)
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var res = await _service.DeleteAsync(userId, eventId);
            return res.Success ? Ok(res) : NotFound(res);
        }
        [HttpPut("events/{eventId:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(int eventId, [FromForm] UpdateEventRequest req)
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var res = await _service.UpdateAsync(userId, eventId, req);
            return res.Success ? Ok(res) : NotFound(res);
        }
    }
}
