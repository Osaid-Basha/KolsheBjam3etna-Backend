using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.DTOs.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KolsheBjam3etna.PL.Areas.User
{
    [Route("api/offers")]
    [ApiController]
    [Authorize]
    public class OffersController : ControllerBase
    {
        private readonly IOfferService _service;
        public OffersController(IOfferService service) => _service = service;

        // ✅ create offer
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOfferRequest req)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var res = await _service.CreateAsync(userId, req);
            return res.Success ? Ok(res) : BadRequest(res);
        }

        // ✅ incoming offers
        [HttpGet("incoming")]
        public async Task<IActionResult> Incoming()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var res = await _service.GetIncomingAsync(userId);
            return Ok(res);
        }

        // ✅ outgoing offers
        [HttpGet("outgoing")]
        public async Task<IActionResult> Outgoing()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var res = await _service.GetOutgoingAsync(userId);
            return Ok(res);
        }

        // ✅ accept
        [HttpPost("{offerId:int}/accept")]
        public async Task<IActionResult> Accept(int offerId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var res = await _service.AcceptAsync(userId, offerId);
            return res.Success ? Ok(res) : BadRequest(res);
        }

        // ✅ reject
        [HttpPost("{offerId:int}/reject")]
        public async Task<IActionResult> Reject(int offerId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var res = await _service.RejectAsync(userId, offerId);
            return res.Success ? Ok(res) : BadRequest(res);
        }
    }
}
