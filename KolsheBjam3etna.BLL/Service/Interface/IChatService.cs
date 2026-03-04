using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.BLL.Service.Interface
{
    public interface IChatService
    {
        Task<int> CreateOrGetDmAsync(string myUserId, string otherUserId);

        Task<List<ChatListItemResponse>> GetMyChatsAsync(string myUserId);

        Task<List<MessageResponse>> GetMessagesAsync(string myUserId, int conversationId, int take = 50, long? beforeId = null);

        Task<MessageResponse> SendMessageAsync(string myUserId, SendMessageRequest request);

        Task<int> MarkReadAsync(string myUserId, int conversationId);
        Task<string> GetReceiverIdAsync(string myUserId, int conversationId);
    }
}
