using KolsheBjam3etna.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Repository.Interface
{
    public interface IConversationRepository
    {
        Task<Conversation?> GetByIdAsync(int id);

        Task<Conversation?> GetDmAsync(string userAId, string userBId);

        Task<Conversation> CreateDmAsync(string userAId, string userBId);

        Task<List<Conversation>> GetUserConversationsAsync(string userId);

        Task SaveChangesAsync();
    }
}
