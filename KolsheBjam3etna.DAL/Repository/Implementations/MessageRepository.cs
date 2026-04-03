using KolsheBjam3etna.DAL.Data;
using KolsheBjam3etna.DAL.Models;
using KolsheBjam3etna.DAL.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace KolsheBjam3etna.DAL.Repository.Class
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _db;

        public MessageRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Message> AddAsync(Message message)
        {
            await _db.Messages.AddAsync(message);
            return message;
        }

        public async Task<List<Message>> GetConversationMessagesAsync(int conversationId, int take = 50, long? beforeId = null)
        {
            var query = _db.Messages
                .Include(m => m.Images)
                .Where(m => m.ConversationId == conversationId)
                .OrderByDescending(m => m.Id)
                .AsQueryable();

            if (beforeId.HasValue)
                query = query.Where(m => m.Id < beforeId.Value);

            return await query.Take(take).ToListAsync();
        }

        public Task<Message?> GetByIdAsync(long messageId)
        {
            return _db.Messages
                .Include(m => m.Images)
                .FirstOrDefaultAsync(m => m.Id == messageId);
        }

        public async Task<int> MarkAsReadAsync(int conversationId, string myUserId)
        {
            var messages = await _db.Messages
                .Where(m => m.ConversationId == conversationId &&
                            m.SenderId != myUserId &&
                            !m.IsRead)
                .ToListAsync();

            foreach (var message in messages)
                message.IsRead = true;

            return messages.Count;
        }

        public Task<int> CountUnreadAsync(int conversationId, string myUserId)
        {
            return _db.Messages.CountAsync(m =>
                m.ConversationId == conversationId &&
                m.SenderId != myUserId &&
                !m.IsRead);
        }

        public Task SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}