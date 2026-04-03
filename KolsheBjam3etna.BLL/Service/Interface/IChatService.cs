using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.DTOs.Response.KolsheBjam3etna.DAL.DTOs.Response;

namespace KolsheBjam3etna.BLL.Service.Interface
{
    public interface IChatService
    {
        Task<int> CreateOrGetDmAsync(string myUserId, string otherUserId);
        Task<List<ChatListItemResponse>> GetMyChatsAsync(string myUserId);
        Task<List<MessageResponse>> GetMessagesAsync(string myUserId, int conversationId, int take = 50, long? beforeId = null);
        Task<MessageResponse> SendMessageAsync(string myUserId, SendMessageRequest request);
        Task<MessageResponse> EditMessageAsync(string myUserId, long messageId, EditMessageRequest request);
        Task<int> DeleteMessageAsync(string myUserId, long messageId);
        Task<MessageResponse> RemoveMessageImageAsync(string myUserId, long messageId, long imageId);
        Task<MessageResponse> RemoveMessageFileAsync(string myUserId, long messageId);
        Task<int> MarkReadAsync(string myUserId, int conversationId);
        Task<string> GetReceiverIdAsync(string myUserId, int conversationId);
        Task<List<string>> GetConversationUserIdsAsync(string myUserId, int conversationId);
    }
}