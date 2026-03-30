using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.DTOs.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KolsheBjam3etna.PL.Controllers
{
    [Route("api/admin/clubs")]
    [ApiController]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminClubsController : ControllerBase
    {
        private readonly IAdminClubsService _service;

        public AdminClubsController(IAdminClubsService service)
        {
            _service = service;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> Summary()
        {
            var res = await _service.GetSummaryAsync();
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search = null, [FromQuery] string? status = null)
        {
            var res = await _service.GetAllAsync(search, status);
            return Ok(res);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _service.GetByIdAsync(id);
            return res.Success ? Ok(res) : NotFound(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClubRequest req)
        {
            var res = await _service.CreateAsync(req);
            return res.Success ? Ok(res) : BadRequest(res);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateClubRequest req)
        {
            var res = await _service.UpdateAsync(id, req);
            return res.Success ? Ok(res) : BadRequest(res);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _service.DeleteAsync(id);
            return res.Success ? Ok(res) : BadRequest(res);
        }

        [HttpPost("{id:int}/renew")]
        public async Task<IActionResult> Renew(int id, [FromBody] RenewClubSubscriptionRequest req)
        {
            var res = await _service.RenewSubscriptionAsync(id, req);
            return res.Success ? Ok(res) : BadRequest(res);
        }
    }
}