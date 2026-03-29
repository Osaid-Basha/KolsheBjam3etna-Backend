using KolsheBjam3etna.BLL.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KolsheBjam3etna.PL.Areas.User
{
    [Route("api/news")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _service;

        public NewsController(INewsService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublished()
        {
            var res = await _service.GetPublishedListAsync();
            return Ok(res);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDetails(int id)
        {
            var res = await _service.GetDetailsAsync(id);
            return res.Success ? Ok(res) : NotFound(res);
        }
    }
}