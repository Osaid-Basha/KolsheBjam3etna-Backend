using KolsheBjam3etna.DAL.Data;
using KolsheBjam3etna.DAL.Models;
using KolsheBjam3etna.DAL.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace KolsheBjam3etna.DAL.Repository.Class
{
    public class MessageImageRepository : IMessageImageRepository
    {
        private readonly ApplicationDbContext _db;

        public MessageImageRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddRangeAsync(List<MessageImage> images)
        {
            await _db.MessageImages.AddRangeAsync(images);
        }

        public Task<MessageImage?> GetByIdAsync(long imageId)
        {
            return _db.MessageImages.FirstOrDefaultAsync(x => x.Id == imageId);
        }

        public Task<List<MessageImage>> GetByMessageIdAsync(long messageId)
        {
            return _db.MessageImages
                .Where(x => x.MessageId == messageId)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();
        }

        public Task DeleteAsync(MessageImage image)
        {
            _db.MessageImages.Remove(image);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}