using KolsheBjam3etna.DAL.Data;
using KolsheBjam3etna.DAL.Models;
using KolsheBjam3etna.DAL.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace KolsheBjam3etna.DAL.Repository.Class
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly ApplicationDbContext _db;
        public ConversationRepository(ApplicationDbContext db) => _db = db;

        private static (string a, string b) Normalize(string userAId, string userBId)
            => string.CompareOrdinal(userAId, userBId) < 0 ? (userAId, userBId) : (userBId, userAId);

        public Task<Conversation?> GetByIdAsync(int id)
            => _db.Conversations.FirstOrDefaultAsync(c => c.Id == id);

        public Task<Conversation?> GetDmAsync(string userAId, string userBId)
        {
            var (a, b) = Normalize(userAId, userBId);
            return _db.Conversations.FirstOrDefaultAsync(c => c.User1Id == a && c.User2Id == b);
        }

        public async Task<Conversation> CreateDmAsync(string userAId, string userBId)
        {
            var (a, b) = Normalize(userAId, userBId);

            var conv = new Conversation
            {
                User1Id = a,
                User2Id = b
            };

            _db.Conversations.Add(conv);
            await _db.SaveChangesAsync();
            return conv;
        }

        public async Task<List<Conversation>> GetUserConversationsAsync(string userId)
        {
            return await _db.Conversations
                .Where(c => c.User1Id == userId || c.User2Id == userId)
                .OrderByDescending(c => c.LastMessageAtUtc ?? c.CreatedAtUtc)
                .ToListAsync();
        }

        public Task SaveChangesAsync() => _db.SaveChangesAsync();
    }
}
