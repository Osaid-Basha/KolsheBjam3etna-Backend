using KolsheBjam3etna.BLL.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KolsheBjam3etna.PL.Areas.User
{
    [Route("api/partner-offers")]
    [ApiController]
    public class PartnerOffersController : ControllerBase
    {
        private readonly IPartnerOfferService _service;

        public PartnerOffersController(IPartnerOfferService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] string? type = null, [FromQuery] string? search = null)
        {
            var res = await _service.GetAllAsync(type, search);
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
