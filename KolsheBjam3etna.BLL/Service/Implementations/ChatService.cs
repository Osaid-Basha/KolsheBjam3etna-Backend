using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using KolsheBjam3etna.DAL.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.BLL.Service.Class
{
    public class ChatService : IChatService
    {
        private readonly IConversationRepository _convRepo;
        private readonly IMessageRepository _msgRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ILocalFileStorageService _localFileStorage;
        private readonly INotificationService _notificationService;

        public ChatService(
            IConversationRepository convRepo,
            IMessageRepository msgRepo,
            UserManager<ApplicationUser> userManager,
            ILocalFileStorageService localFileStorage,
              INotificationService notificationService
        )
        {
            _convRepo = convRepo;
            _msgRepo = msgRepo;
            _userManager = userManager;
            _localFileStorage = localFileStorage;
            _notificationService = notificationService;
        }

        public async Task<int> CreateOrGetDmAsync(string myUserId, string otherUserId)
        {
            if (string.IsNullOrWhiteSpace(myUserId))
                throw new Exception("Invalid user (missing myUserId).");

            if (string.IsNullOrWhiteSpace(otherUserId) || myUserId == otherUserId)
                throw new Exception("Invalid other user.");

            var me = await _userManager.FindByIdAsync(myUserId);
            if (me == null) throw new Exception("My user not found.");

            var other = await _userManager.FindByIdAsync(otherUserId);
            if (other == null) throw new Exception("Other user not found.");

            var existing = await _convRepo.GetDmAsync(myUserId, otherUserId);
            if (existing != null) return existing.Id;

            var created = await _convRepo.CreateDmAsync(myUserId, otherUserId);
            return created.Id;
        }

        public async Task<List<ChatListItemResponse>> GetMyChatsAsync(string myUserId)
        {
            var convs = await _convRepo.GetUserConversationsAsync(myUserId);

            var list = new List<ChatListItemResponse>();

            foreach (var c in convs)
            {
                var otherId = (c.User1Id == myUserId) ? c.User2Id : c.User1Id;
                var other = await _userManager.FindByIdAsync(otherId);

                var unread = await _msgRepo.CountUnreadAsync(c.Id, myUserId);

                list.Add(new ChatListItemResponse
                {
                    ConversationId = c.Id,
                    OtherUserId = otherId,
                    OtherFullName = other?.FullName ?? "",
                    OtherProfileImageUrl = other?.ProfileImageUrl,
                    LastMessageText = c.LastMessageText,
                    LastMessageAtUtc = c.LastMessageAtUtc,
                    UnreadCount = unread
                });
            }

            return list;
        }

        public async Task<List<MessageResponse>> GetMessagesAsync(string myUserId, int conversationId, int take = 50, long? beforeId = null)
        {
            var conv = await _convRepo.GetByIdAsync(conversationId);
            if (conv == null) throw new Exception("Conversation not found.");

        
            if (conv.User1Id != myUserId && conv.User2Id != myUserId)
                throw new Exception("Forbidden.");

            var msgs = await _msgRepo.GetConversationMessagesAsync(conversationId, take, beforeId);

          
            msgs.Reverse();

            return msgs.Select(m => new MessageResponse
            {
                Id = m.Id,
                ConversationId = m.ConversationId,
                SenderId = m.SenderId,
                Text = m.Text,
                SentAtUtc = m.SentAtUtc,
                IsRead = m.IsRead
            }).ToList();
        }

        public async Task<MessageResponse> SendMessageAsync(string myUserId, SendMessageRequest request)
        {
            var conv = await _convRepo.GetByIdAsync(request.ConversationId);
            if (conv == null) throw new Exception("Conversation not found.");

            if (conv.User1Id != myUserId && conv.User2Id != myUserId)
                throw new Exception("Forbidden.");

            if (string.IsNullOrWhiteSpace(request.Text) && request.Image == null)
                throw new Exception("Message must have text or image");

            string? imageUrl = null;
            MessageType type = MessageType.Text;

            if (request.Image != null)
            {
                imageUrl = await _localFileStorage.SaveChatImageAsync(request.Image);
                type = MessageType.Image;
            }

            var msg = new Message
            {
                ConversationId = request.ConversationId,
                SenderId = myUserId,
                Type = type,
                Text = request.Text,
                ImageUrl = imageUrl,
                SentAtUtc = DateTime.UtcNow,
                IsRead = false
            };

            var saved = await _msgRepo.AddAsync(msg);

            // last message
            conv.LastMessageText = type == MessageType.Image
                ? "[Image]"
                : saved.Text;

            conv.LastMessageAtUtc = saved.SentAtUtc;
            await _convRepo.SaveChangesAsync();

            var receiverId = conv.User1Id == myUserId ? conv.User2Id : conv.User1Id;

            var me = await _userManager.FindByIdAsync(myUserId);
            var senderName = me?.FullName ?? "مستخدم";

            await _notificationService.CreateAsync(
                receiverId,
                type == MessageType.Image ? "صورة جديدة" : "رسالة جديدة",
                type == MessageType.Image
                    ? $"صورة جديدة من {senderName}"
                    : $"رسالة جديدة من {senderName}",
                "Message",
                targetType: "Chat",
                targetId: conv.Id
            );

            return new MessageResponse
            {
                Id = saved.Id,
                ConversationId = saved.ConversationId,
                SenderId = saved.SenderId,
                Text = saved.Text,
                SentAtUtc = saved.SentAtUtc,
                IsRead = saved.IsRead
            };
        }

        public async Task<int> MarkReadAsync(string myUserId, int conversationId)
        {
            var conv = await _convRepo.GetByIdAsync(conversationId);
            if (conv == null) throw new Exception("Conversation not found.");

            if (conv.User1Id != myUserId && conv.User2Id != myUserId)
                throw new Exception("Forbidden.");

        
            return await _msgRepo.MarkAsReadAsync(conversationId, myUserId);
        }
        public async Task<string> GetReceiverIdAsync(string myUserId, int conversationId)
        {
            var conv = await _convRepo.GetByIdAsync(conversationId);
            if (conv == null) throw new Exception("Conversation not found.");

            if (conv.User1Id != myUserId && conv.User2Id != myUserId)
                throw new Exception("Forbidden.");

            return conv.User1Id == myUserId ? conv.User2Id : conv.User1Id;
        }
        public async Task<Message> SendImageAsync(string senderId, int conversationId, IFormFile image, string? caption)
        {
            var conv = await _convRepo.GetByIdAsync(conversationId);
            if (conv == null) throw new Exception("Conversation not found.");

            if (conv.User1Id != senderId && conv.User2Id != senderId)
                throw new Exception("Forbidden.");

            var imageUrl = await _localFileStorage.SaveChatImageAsync(image);
            if (imageUrl == null) throw new Exception("Upload failed");

            var msg = new Message
            {
                ConversationId = conversationId,
                SenderId = senderId,
                Type = MessageType.Image,
                ImageUrl = imageUrl,
                Text = caption,
                SentAtUtc = DateTime.UtcNow,
                IsRead = false
            };

            await _msgRepo.AddAsync(msg);
            await _msgRepo.SaveChangesAsync();

            // update last message
            conv.LastMessageText = "[Image]";
            conv.LastMessageAtUtc = msg.SentAtUtc;
            await _convRepo.SaveChangesAsync();

            // notification
            var receiverId = conv.User1Id == senderId ? conv.User2Id : conv.User1Id;
            var me = await _userManager.FindByIdAsync(senderId);
            var senderName = me?.FullName ?? "مستخدم";

            await _notificationService.CreateAsync(
                receiverId,
                "صورة جديدة",
                $"صورة جديدة من {senderName}",
                "Message",
                targetType: "Chat",
                targetId: conversationId
            );

            return msg;
        }
    }
}
