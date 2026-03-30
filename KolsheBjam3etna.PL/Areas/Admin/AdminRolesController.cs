using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.DTOs.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/admin/roles")]
[ApiController]
[Authorize(Roles = "Admin,SuperAdmin")]
public class AdminRolesController : ControllerBase
{
    private readonly IAdminRolesService _service;

    public AdminRolesController(IAdminRolesService service)
    {
        _service = service;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> Summary()
    {
        var res = await _service.GetRolesSummaryAsync();
        return Ok(res);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search = null, [FromQuery] string? role = null)
    {
        var res = await _service.GetAllRoleUsersAsync(search, role);
        return Ok(res);
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetUserRole([FromQuery] string email)
    {
        var res = await _service.GetUserRoleAsync(email);
        return res.Success ? Ok(res) : NotFound(res);
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableRoles()
    {
        var res = await _service.GetAvailableRolesAsync();
        return Ok(res);
    }

    [HttpGet("clubs")]
    public async Task<IActionResult> GetClubs()
    {
        var res = await _service.GetClubOptionsAsync();
        return Ok(res);
    }

    [HttpPost("assign")]
    public async Task<IActionResult> Assign([FromBody] AssignRoleByEmailRequest req)
    {
        var res = await _service.AssignRoleAsync(req);
        return res.Success ? Ok(res) : BadRequest(res);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateRoleByEmailRequest req)
    {
        var res = await _service.UpdateRoleAsync(req);
        return res.Success ? Ok(res) : BadRequest(res);
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> Remove([FromBody] RemoveRoleByEmailRequest req)
    {
        var res = await _service.RemoveRoleAsync(req);
        return res.Success ? Ok(res) : BadRequest(res);
    }
}