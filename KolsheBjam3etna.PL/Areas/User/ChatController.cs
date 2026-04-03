using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.PL.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

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
            var conversationId = await _chat.CreateOrGetDmAsync(MyId, request.OtherUserId);
            return Ok(new { conversationId });
        }

        [HttpGet("list")]
        public async Task<IActionResult> List()
        {
            var chats = await _chat.GetMyChatsAsync(MyId);
            return Ok(chats);
        }

        [HttpGet("{conversationId:int}/messages")]
        public async Task<IActionResult> Messages(int conversationId, [FromQuery] int take = 50, [FromQuery] long? beforeId = null)
        {
            var messages = await _chat.GetMessagesAsync(MyId, conversationId, take, beforeId);
            return Ok(messages);
        }

        [HttpPost("send")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Send([FromForm] SendMessageRequest request)
        {
            var message = await _chat.SendMessageAsync(MyId, request);
            await BroadcastToConversationUsers(message.ConversationId, "ReceiveMessage", message);
            return Ok(message);
        }

        [HttpPut("messages/{messageId:long}")]
        public async Task<IActionResult> EditMessage(long messageId, [FromBody] EditMessageRequest request)
        {
            var message = await _chat.EditMessageAsync(MyId, messageId, request);
            await BroadcastToConversationUsers(message.ConversationId, "MessageUpdated", message);
            return Ok(message);
        }

        [HttpDelete("messages/{messageId:long}")]
        public async Task<IActionResult> DeleteMessage(long messageId)
        {
            var conversationId = await _chat.DeleteMessageAsync(MyId, messageId);
            await BroadcastToConversationUsers(conversationId, "MessageDeleted", new { messageId });
            return Ok(new { message = "Message deleted" });
        }

        [HttpDelete("messages/{messageId:long}/file")]
        public async Task<IActionResult> RemoveFile(long messageId)
        {
            var message = await _chat.RemoveMessageFileAsync(MyId, messageId);
            await BroadcastToConversationUsers(message.ConversationId, "MessageUpdated", message);
            return Ok(message);
        }

        [HttpDelete("messages/{messageId:long}/images/{imageId:long}")]
        public async Task<IActionResult> RemoveImage(long messageId, long imageId)
        {
            var message = await _chat.RemoveMessageImageAsync(MyId, messageId, imageId);
            await BroadcastToConversationUsers(message.ConversationId, "MessageUpdated", message);
            return Ok(message);
        }

        [HttpPost("{conversationId:int}/read")]
        public async Task<IActionResult> MarkRead(int conversationId)
        {
            var count = await _chat.MarkReadAsync(MyId, conversationId);
            return Ok(new { marked = count });
        }

        private async Task BroadcastToConversationUsers(int conversationId, string eventName, object payload)
        {
            var userIds = await _chat.GetConversationUserIdsAsync(MyId, conversationId);

            foreach (var userId in userIds)
            {
                await _hub.Clients.Group($"user:{userId}")
                    .SendAsync(eventName, payload);
            }
        }
    }
}