using KolsheBjam3etna.DAL.Models;

namespace KolsheBjam3etna.DAL.Repository.Interface
{
    public interface IMessageImageRepository
    {
        Task AddRangeAsync(List<MessageImage> images);
        Task<MessageImage?> GetByIdAsync(long imageId);
        Task<List<MessageImage>> GetByMessageIdAsync(long messageId);
        Task DeleteAsync(MessageImage image);
        Task SaveChangesAsync();
    }
}