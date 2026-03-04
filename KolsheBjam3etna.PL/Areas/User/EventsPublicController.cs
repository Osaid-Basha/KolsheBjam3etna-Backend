using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.DTOs.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class EventsPublicController : ControllerBase
{
    private readonly IEventPublicService _service;
    public EventsPublicController(IEventPublicService service) => _service = service;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] string? search = null, [FromQuery] string? type = null)
    {
        var res = await _service.GetAllAsync(search, type);
        return Ok(res);
    }

   
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDetails(int id)
    {
        var res = await _service.GetDetailsAsync(id);
        return res.Success ? Ok(res) : NotFound(res);
    }

   
    [HttpPost("{id:int}/register")]
    [Authorize]
    public async Task<IActionResult> Register(int id, [FromBody] RegisterEventRequest req)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var res = await _service.RegisterAsync(id, userId, req);
        return res.Success ? Ok(res) : BadRequest(res);
    }
}