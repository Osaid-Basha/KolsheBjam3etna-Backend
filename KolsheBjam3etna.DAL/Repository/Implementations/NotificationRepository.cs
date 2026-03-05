using KolsheBjam3etna.DAL.Data;
using KolsheBjam3etna.DAL.Models;
using KolsheBjam3etna.DAL.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace KolsheBjam3etna.DAL.Repository.Implementations
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _db;
        public NotificationRepository(ApplicationDbContext db) => _db = db;

        public async Task AddAsync(Notification n) => await _db.Notifications.AddAsync(n);
        public async Task SaveAsync() => await _db.SaveChangesAsync();

        public Task<List<Notification>> GetMyAsync(string userId, int take = 30) =>
            _db.Notifications.AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Take(take)
                .ToListAsync();

        public Task<int> GetUnreadCountAsync(string userId) =>
            _db.Notifications.CountAsync(x => x.UserId == userId && !x.IsRead);

        public Task<Notification?> GetByIdAsync(int id, string userId) =>
            _db.Notifications.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        public Task<List<Notification>> GetMyTrackingAsync(string userId, int take = 200) =>
    _db.Notifications
        .Where(x => x.UserId == userId && !x.IsRead)
        .OrderByDescending(x => x.CreatedAtUtc)
        .Take(take)
        .ToListAsync();
    }
}
