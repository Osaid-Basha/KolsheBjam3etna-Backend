using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.DTOs.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KolsheBjam3etna.PL.Areas.Admin
{
    [Route("api/admin/news")]
    [ApiController]
    [Authorize(Roles = "Admin,SuperAdmin,NewsEditor")]
    public class AdminNewsController : ControllerBase
    {
        private readonly INewsService _service;

        public AdminNewsController(INewsService service)
        {
            _service = service;
        }

        
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateNewsRequest req)
        {
            var res = await _service.CreateAsync(req);
            return res.Success ? Ok(res) : BadRequest(res);
        }

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateNewsRequest req)
        {
            var res = await _service.UpdateAsync(id, req);
            return res.Success ? Ok(res) : NotFound(res);
        }

        [HttpPost("{id:int}/publish")]
        public async Task<IActionResult> Publish(int id)
        {
            var res = await _service.PublishAsync(id);
            return res.Success ? Ok(res) : NotFound(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var res = await _service.GetAdminListAsync();
            return Ok(res);
        }
        [HttpPost("{id:int}/unpublish")]
        public async Task<IActionResult> Unpublish(int id)
        {
            var res = await _service.UnpublishAsync(id);

            return res.Success ? Ok(res) : NotFound(res);
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _service.RemoveAsync(id);
            return res.Success ? Ok(res) : NotFound(res);
        }
    }
}
