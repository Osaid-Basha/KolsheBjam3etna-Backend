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
    public AdminRolesController(IAdminRolesService service) => _service = service;

   
    [HttpGet]
    public async Task<IActionResult> Summary()
    {
        var res = await _service.GetRolesSummaryAsync();
        return Ok(res);
    }

 
    [HttpGet("user")]
    public async Task<IActionResult> UserRoles([FromQuery] string email)
    {
        var res = await _service.GetUserRolesAsync(email);
        return res.Success ? Ok(res) : NotFound(res);
    }


    [HttpPost("assign")]
    public async Task<IActionResult> Assign([FromBody] AssignRoleByEmailRequest req)
    {
        var res = await _service.AssignRoleAsync(req);
        return res.Success ? Ok(res) : BadRequest(res);
    }

  
    [HttpPost("remove")]
    public async Task<IActionResult> Remove([FromBody] RemoveRoleByEmailRequest req)
    {
        var res = await _service.RemoveRoleAsync(req);
        return res.Success ? Ok(res) : BadRequest(res);
    }
}