using KolsheBjam3etna.DAL.Data;
using KolsheBjam3etna.DAL.Models;
using KolsheBjam3etna.DAL.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
namespace KolsheBjam3etna.DAL.Repository.Class
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _db;
        public MessageRepository(ApplicationDbContext db) => _db = db;

        public async Task<Message> AddAsync(Message message)
        {
            _db.Messages.Add(message);
            await _db.SaveChangesAsync();
            return message;
        }

        public async Task<List<Message>> GetConversationMessagesAsync(int conversationId, int take = 50, long? beforeId = null)
        {
            var q = _db.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderByDescending(m => m.Id)
                .AsQueryable();

            if (beforeId.HasValue)
                q = q.Where(m => m.Id < beforeId.Value);

            return await q.Take(take).ToListAsync();
        }

        public async Task<int> MarkAsReadAsync(int conversationId, string myUserId)
        {
            var msgs = await _db.Messages
                .Where(m => m.ConversationId == conversationId && m.SenderId != myUserId && !m.IsRead)
                .ToListAsync();

            foreach (var m in msgs) m.IsRead = true;

            await _db.SaveChangesAsync();
            return msgs.Count;
        }

        public Task<int> CountUnreadAsync(int conversationId, string myUserId)
        {
            return _db.Messages.CountAsync(m =>
                m.ConversationId == conversationId &&
                m.SenderId != myUserId &&
                !m.IsRead);
        }

        public Task SaveChangesAsync() => _db.SaveChangesAsync();
    }
}
