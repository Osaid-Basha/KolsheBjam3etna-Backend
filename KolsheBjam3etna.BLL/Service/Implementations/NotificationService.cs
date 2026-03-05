using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using KolsheBjam3etna.DAL.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.BLL.Service.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;

        public NotificationService(INotificationRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<NotificationsSummaryDto>> GetMyAsync(string userId, int take = 30)
        {
            var items = await _repo.GetMyAsync(userId, take);
            var unread = await _repo.GetUnreadCountAsync(userId);

            var dto = new NotificationsSummaryDto
            {
                UnreadCount = unread,
                Items = items.Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Type = n.Type.ToString(),
                    Title = n.Title,
                    Body = n.Body,
                    IsRead = n.IsRead,
                    CreatedAtUtc = n.CreatedAtUtc,
                    TargetType = n.TargetType,
                    TargetId = n.TargetId
                }).ToList()
            };

            return ApiResponse<NotificationsSummaryDto>.Ok(dto, "Success");
        }

        public async Task<ApiResponse<int>> GetUnreadCountAsync(string userId)
        {
            var c = await _repo.GetUnreadCountAsync(userId);
            return ApiResponse<int>.Ok(c, "Success");
        }

        public async Task<ApiResponse<string>> MarkReadAsync(string userId, int notificationId)
        {
            var n = await _repo.GetByIdAsync(notificationId, userId);
            if (n == null) return ApiResponse<string>.Fail("Not found");

            n.IsRead = true;
            await _repo.SaveAsync();
            return ApiResponse<string>.Ok("Read", "Marked as read");
        }

        public async Task<ApiResponse<string>> MarkAllReadAsync(string userId)
        {
            var items = await _repo.GetMyTrackingAsync(userId, 200);
            foreach (var n in items) n.IsRead = true;

            await _repo.SaveAsync();
            return ApiResponse<string>.Ok("Read", "All notifications marked as read");
        }

        public async Task CreateAsync(string userId, string title, string body, string type, string? targetType = null, int? targetId = null)
        {
           
            if (!Enum.TryParse<NotificationType>(type, true, out var nt))
                nt = NotificationType.Announcement;

            var n = new Notification
            {
                UserId = userId,
                Type = nt,
                Title = title,
                Body = body,
                TargetType = targetType,
                TargetId = targetId,
                IsRead = false
            };

            await _repo.AddAsync(n);
            await _repo.SaveAsync();
        }
       
    }
}
