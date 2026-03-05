using KolsheBjam3etna.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Repository.Interface
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification n);
        Task SaveAsync();

        Task<List<Notification>> GetMyAsync(string userId, int take = 30);
        Task<int> GetUnreadCountAsync(string userId);

        Task<Notification?> GetByIdAsync(int id, string userId);
        Task<List<Notification>> GetMyTrackingAsync(string userId, int take = 200);
    }
}
