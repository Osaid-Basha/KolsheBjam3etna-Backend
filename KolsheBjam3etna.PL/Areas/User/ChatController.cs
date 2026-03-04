using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.DTOs.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using KolsheBjam3etna.PL.Hubs;

namespace KolsheBjam3etna.PL.Areas.Identity
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chat;
        private readonly IHubContext<ChatHub> _hub;

        public ChatController(IChatService chat, IHubContext<ChatHub> hub)
        {
            _chat = chat;
            _hub = hub;
        }

        private string MyId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpPost("dm")]
        public async Task<IActionResult> CreateDm([FromBody] CreateDmRequest request)
        {
            var myUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine($"myUserId = {myUserId}");
            Console.WriteLine($"otherUserId = {request.OtherUserId}");
            if (string.IsNullOrWhiteSpace(myUserId))
                return Unauthorized(new { message = "Missing user id in token" });

            var id = await _chat.CreateOrGetDmAsync(myUserId, request.OtherUserId);
            return Ok(new { conversationId = id });
        }

        [HttpGet("list")]
        public async Task<IActionResult> List()
            => Ok(await _chat.GetMyChatsAsync(MyId));

        [HttpGet("{conversationId:int}/messages")]
        public async Task<IActionResult> Messages(int conversationId, [FromQuery] int take = 50, [FromQuery] long? beforeId = null)
            => Ok(await _chat.GetMessagesAsync(MyId, conversationId, take, beforeId));

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SendMessageRequest request)
        {
            var myId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var msg = await _chat.SendMessageAsync(myId, request);

            var receiverId = await _chat.GetReceiverIdAsync(myId, request.ConversationId);

         
            await _hub.Clients.Group($"user:{myId}")
                .SendAsync("ReceiveMessage", msg);

     
            await _hub.Clients.Group($"user:{receiverId}")
                .SendAsync("ReceiveMessage", msg);

            return Ok(msg);
        }

        [HttpPost("{conversationId:int}/read")]
        public async Task<IActionResult> MarkRead(int conversationId)
        {
            var count = await _chat.MarkReadAsync(MyId, conversationId);
            return Ok(new { marked = count });
        }
    }
}