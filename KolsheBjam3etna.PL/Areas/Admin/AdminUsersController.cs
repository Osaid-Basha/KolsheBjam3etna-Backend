using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.DTOs.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KolsheBjam3etna.PL.Controllers
{
    [Route("api/admin/users")]
    [ApiController]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminUsersController : ControllerBase
    {
        private readonly IAdminService _service;

        public AdminUsersController(IAdminService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search = null,
            [FromQuery] string? status = null)
        {
            var res = await _service.GetUsersAsync(search, status);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var res = await _service.GetUserAsync(id);
            return res.Success ? Ok(res) : NotFound(res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] AdminUpdateUserDto dto)
        {
            var res = await _service.UpdateUserAsync(id, dto);
            return res.Success ? Ok(res) : NotFound(res);
        }

        [HttpPost("{id}/block")]
        public async Task<IActionResult> Block(string id)
        {
            var res = await _service.BlockAsync(id);
            return res.Success ? Ok(res) : NotFound(res);
        }

        [HttpPost("{id}/unblock")]
        public async Task<IActionResult> Unblock(string id)
        {
            var res = await _service.UnblockAsync(id);
            return res.Success ? Ok(res) : NotFound(res);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var res = await _service.DeleteUserAsync(id);
            return res.Success ? Ok(res) : NotFound(res);
        }
    }
}