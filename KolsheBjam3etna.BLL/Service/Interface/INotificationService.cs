using KolsheBjam3etna.DAL.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.BLL.Service.Interface
{
    public interface INotificationService
    {
        Task<ApiResponse<NotificationsSummaryDto>> GetMyAsync(string userId, int take = 30);
        Task<ApiResponse<int>> GetUnreadCountAsync(string userId);

        Task<ApiResponse<string>> MarkReadAsync(string userId, int notificationId);
        Task<ApiResponse<string>> MarkAllReadAsync(string userId);

        Task CreateAsync(string userId, string title, string body, string type, string? targetType = null, int? targetId = null);
    }
}
