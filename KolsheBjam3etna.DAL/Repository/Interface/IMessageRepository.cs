using KolsheBjam3etna.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Repository.Interface
{
    public interface IMessageRepository
    {
        Task<Message> AddAsync(Message message);

        Task<List<Message>> GetConversationMessagesAsync(int conversationId, int take = 50, long? beforeId = null);

        Task<int> MarkAsReadAsync(int conversationId, string myUserId);

        Task<int> CountUnreadAsync(int conversationId, string myUserId);

        Task SaveChangesAsync();
    }
}
