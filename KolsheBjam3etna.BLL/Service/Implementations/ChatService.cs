using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.DTOs.Response.KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using KolsheBjam3etna.DAL.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace KolsheBjam3etna.BLL.Service.Class
{
    public class ChatService : IChatService
    {
        private readonly IConversationRepository _convRepo;
        private readonly IMessageRepository _msgRepo;
        private readonly IMessageImageRepository _messageImageRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILocalFileStorageService _localFileStorage;
        private readonly INotificationService _notificationService;

        public ChatService(
            IConversationRepository convRepo,
            IMessageRepository msgRepo,
            IMessageImageRepository messageImageRepo,
            UserManager<ApplicationUser> userManager,
            ILocalFileStorageService localFileStorage,
            INotificationService notificationService)
        {
            _convRepo = convRepo;
            _msgRepo = msgRepo;
            _messageImageRepo = messageImageRepo;
            _userManager = userManager;
            _localFileStorage = localFileStorage;
            _notificationService = notificationService;
        }

        public async Task<int> CreateOrGetDmAsync(string myUserId, string otherUserId)
        {
            ValidateDmUsers(myUserId, otherUserId);

            var me = await _userManager.FindByIdAsync(myUserId)
                     ?? throw new Exception("My user not found.");

            var other = await _userManager.FindByIdAsync(otherUserId)
                        ?? throw new Exception("Other user not found.");

            var existing = await _convRepo.GetDmAsync(myUserId, otherUserId);
            if (existing != null)
                return existing.Id;

            var created = await _convRepo.CreateDmAsync(myUserId, otherUserId);
            return created.Id;
        }

        public async Task<List<ChatListItemResponse>> GetMyChatsAsync(string myUserId)
        {
            var conversations = await _convRepo.GetUserConversationsAsync(myUserId);
            var list = new List<ChatListItemResponse>();

            foreach (var conversation in conversations)
            {
                var otherUserId = conversation.User1Id == myUserId ? conversation.User2Id : conversation.User1Id;
                var otherUser = await _userManager.FindByIdAsync(otherUserId);
                var unreadCount = await _msgRepo.CountUnreadAsync(conversation.Id, myUserId);

                list.Add(new ChatListItemResponse
                {
                    ConversationId = conversation.Id,
                    OtherUserId = otherUserId,
                    OtherFullName = otherUser?.FullName ?? string.Empty,
                    OtherProfileImageUrl = otherUser?.ProfileImageUrl,
                    LastMessageText = conversation.LastMessageText,
                    LastMessageAtUtc = conversation.LastMessageAtUtc,
                    UnreadCount = unreadCount
                });
            }

            return list;
        }

        public async Task<List<MessageResponse>> GetMessagesAsync(string myUserId, int conversationId, int take = 50, long? beforeId = null)
        {
            var conversation = await ValidateConversationAccessAsync(myUserId, conversationId);
            var messages = await _msgRepo.GetConversationMessagesAsync(conversation.Id, take, beforeId);

            messages.Reverse();
            return messages.Select(MapMessageResponse).ToList();
        }

        public async Task<MessageResponse> SendMessageAsync(string myUserId, SendMessageRequest request)
        {
            var conversation = await ValidateConversationAccessAsync(myUserId, request.ConversationId);

            var hasText = !string.IsNullOrWhiteSpace(request.Text);
            var hasImages = request.Images != null && request.Images.Any();
            var hasFile = request.File != null;

            if (!hasText && !hasImages && !hasFile)
                throw new Exception("Message must have text, image, or file.");

            var message = new Message
            {
                ConversationId = request.ConversationId,
                SenderId = myUserId,
                Text = request.Text,
                SentAtUtc = DateTime.UtcNow,
                IsRead = false,
                IsEdited = false,
                IsDeleted = false
            };

            if (hasFile)
                await FillMessageFileDataAsync(message, request.File!);

            await _msgRepo.AddAsync(message);
            await _msgRepo.SaveChangesAsync();

            if (hasImages)
                await AddMessageImagesAsync(message, request.Images!);

            message.Type = ResolveMessageType(message.Text, message.Images.Any(), message.FileUrl);
            await _msgRepo.SaveChangesAsync();

            await UpdateConversationLastMessageAsync(conversation, message);

            await CreateNewMessageNotificationAsync(conversation, myUserId);

            return MapMessageResponse(message);
        }

        public async Task<MessageResponse> EditMessageAsync(string myUserId, long messageId, EditMessageRequest request)
        {
            var message = await GetOwnedActiveMessageAsync(myUserId, messageId);

            var willBeEmpty =
                string.IsNullOrWhiteSpace(request.Text) &&
                !message.Images.Any() &&
                string.IsNullOrWhiteSpace(message.FileUrl);

            if (willBeEmpty)
                throw new Exception("Message cannot be empty.");

            message.Text = request.Text;
            message.IsEdited = true;
            message.EditedAtUtc = DateTime.UtcNow;
            message.Type = ResolveMessageType(message.Text, message.Images.Any(), message.FileUrl);

            await _msgRepo.SaveChangesAsync();
            await RefreshConversationPreviewAsync(message.ConversationId);

            return MapMessageResponse(message);
        }

        public async Task<int> DeleteMessageAsync(string myUserId, long messageId)
        {
            var message = await GetOwnedMessageAsync(myUserId, messageId);
            var conversationId = message.ConversationId;

            await DeleteMessageAssetsAsync(message);

            message.Text = null;
            message.FileUrl = null;
            message.FileName = null;
            message.FileSize = null;
            message.FileContentType = null;
            message.IsDeleted = true;
            message.DeletedAtUtc = DateTime.UtcNow;
            message.IsEdited = false;
            message.EditedAtUtc = null;

            await _msgRepo.SaveChangesAsync();
            await RefreshConversationPreviewAsync(conversationId);

            return conversationId;
        }

        public async Task<MessageResponse> RemoveMessageImageAsync(string myUserId, long messageId, long imageId)
        {
            var message = await GetOwnedActiveMessageAsync(myUserId, messageId);

            var image = message.Images.FirstOrDefault(x => x.Id == imageId)
                        ?? throw new Exception("Image not found.");

            var willBeEmpty =
                string.IsNullOrWhiteSpace(message.Text) &&
                string.IsNullOrWhiteSpace(message.FileUrl) &&
                message.Images.Count == 1;

            if (willBeEmpty)
                throw new Exception("Cannot remove image because message would become empty.");

            await _localFileStorage.DeleteFileAsync(image.ImageUrl);
            message.Images.Remove(image);
            await _messageImageRepo.DeleteAsync(image);

            ReorderMessageImages(message);

            message.IsEdited = true;
            message.EditedAtUtc = DateTime.UtcNow;
            message.Type = ResolveMessageType(message.Text, message.Images.Any(), message.FileUrl);

            await _messageImageRepo.SaveChangesAsync();
            await _msgRepo.SaveChangesAsync();
            await RefreshConversationPreviewAsync(message.ConversationId);

            return MapMessageResponse(message);
        }

        public async Task<MessageResponse> RemoveMessageFileAsync(string myUserId, long messageId)
        {
            var message = await GetOwnedActiveMessageAsync(myUserId, messageId);

            if (string.IsNullOrWhiteSpace(message.FileUrl))
                throw new Exception("Message has no file.");

            var willBeEmpty =
                string.IsNullOrWhiteSpace(message.Text) &&
                !message.Images.Any();

            if (willBeEmpty)
                throw new Exception("Cannot remove file because message would become empty.");

            await _localFileStorage.DeleteFileAsync(message.FileUrl);

            message.FileUrl = null;
            message.FileName = null;
            message.FileSize = null;
            message.FileContentType = null;
            message.Type = ResolveMessageType(message.Text, message.Images.Any(), message.FileUrl);
            message.IsEdited = true;
            message.EditedAtUtc = DateTime.UtcNow;

            await _msgRepo.SaveChangesAsync();
            await RefreshConversationPreviewAsync(message.ConversationId);

            return MapMessageResponse(message);
        }

        public async Task<int> MarkReadAsync(string myUserId, int conversationId)
        {
            await ValidateConversationAccessAsync(myUserId, conversationId);
            var count = await _msgRepo.MarkAsReadAsync(conversationId, myUserId);
            await _msgRepo.SaveChangesAsync();
            return count;
        }

        public async Task<string> GetReceiverIdAsync(string myUserId, int conversationId)
        {
            var conversation = await ValidateConversationAccessAsync(myUserId, conversationId);
            return conversation.User1Id == myUserId ? conversation.User2Id : conversation.User1Id;
        }

        public async Task<List<string>> GetConversationUserIdsAsync(string myUserId, int conversationId)
        {
            var conversation = await ValidateConversationAccessAsync(myUserId, conversationId);
            return new List<string> { conversation.User1Id, conversation.User2Id };
        }

        private async Task<Conversation> ValidateConversationAccessAsync(string myUserId, int conversationId)
        {
            var conversation = await _convRepo.GetByIdAsync(conversationId)
                               ?? throw new Exception("Conversation not found.");

            if (conversation.User1Id != myUserId && conversation.User2Id != myUserId)
                throw new Exception("Forbidden.");

            return conversation;
        }

        private async Task<Message> GetOwnedMessageAsync(string myUserId, long messageId)
        {
            var message = await _msgRepo.GetByIdAsync(messageId)
                          ?? throw new Exception("Message not found.");

            if (message.SenderId != myUserId)
                throw new Exception("You can only modify your own messages.");

            return message;
        }

        private async Task<Message> GetOwnedActiveMessageAsync(string myUserId, long messageId)
        {
            var message = await GetOwnedMessageAsync(myUserId, messageId);

            if (message.IsDeleted)
                throw new Exception("Deleted message cannot be modified.");

            return message;
        }

        private async Task FillMessageFileDataAsync(Message message, IFormFile file)
        {
            message.FileUrl = await _localFileStorage.SaveChatFileAsync(file);
            message.FileName = file.FileName;
            message.FileSize = file.Length;
            message.FileContentType = file.ContentType;
        }

        private async Task AddMessageImagesAsync(Message message, IEnumerable<IFormFile> images)
        {
            var list = new List<MessageImage>();
            var index = 0;

            foreach (var image in images)
            {
                var imageUrl = await _localFileStorage.SaveChatImageAsync(image);
                if (string.IsNullOrWhiteSpace(imageUrl))
                    continue;

                list.Add(new MessageImage
                {
                    MessageId = message.Id,
                    ImageUrl = imageUrl,
                    SortOrder = index++
                });
            }

            if (!list.Any())
                return;

            await _messageImageRepo.AddRangeAsync(list);
            await _messageImageRepo.SaveChangesAsync();

            message.Images = list;
        }

        private async Task DeleteMessageAssetsAsync(Message message)
        {
            foreach (var image in message.Images.ToList())
            {
                await _localFileStorage.DeleteFileAsync(image.ImageUrl);
                await _messageImageRepo.DeleteAsync(image);
            }

            await _messageImageRepo.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(message.FileUrl))
                await _localFileStorage.DeleteFileAsync(message.FileUrl);
        }

        private async Task UpdateConversationLastMessageAsync(Conversation conversation, Message message)
        {
            conversation.LastMessageText = BuildConversationLastMessage(message);
            conversation.LastMessageAtUtc = message.SentAtUtc;
            await _convRepo.SaveChangesAsync();
        }

        private async Task RefreshConversationPreviewAsync(int conversationId)
        {
            var conversation = await _convRepo.GetByIdAsync(conversationId);
            if (conversation == null)
                return;

            var latestMessages = await _msgRepo.GetConversationMessagesAsync(conversationId, 1);
            var latestMessage = latestMessages.FirstOrDefault();

            if (latestMessage == null)
            {
                conversation.LastMessageText = string.Empty;
                conversation.LastMessageAtUtc = null;
            }
            else
            {
                conversation.LastMessageText = BuildConversationLastMessage(latestMessage);
                conversation.LastMessageAtUtc = latestMessage.SentAtUtc;
            }

            await _convRepo.SaveChangesAsync();
        }

        private async Task CreateNewMessageNotificationAsync(Conversation conversation, string senderId)
        {
            var receiverId = conversation.User1Id == senderId ? conversation.User2Id : conversation.User1Id;
            var sender = await _userManager.FindByIdAsync(senderId);
            var senderName = sender?.FullName ?? "مستخدم";

            await _notificationService.CreateAsync(
                receiverId,
                "رسالة جديدة",
                $"رسالة جديدة من {senderName}",
                "Message",
                targetType: "Chat",
                targetId: conversation.Id
            );
        }

        private static void ValidateDmUsers(string myUserId, string otherUserId)
        {
            if (string.IsNullOrWhiteSpace(myUserId))
                throw new Exception("Invalid user.");

            if (string.IsNullOrWhiteSpace(otherUserId) || myUserId == otherUserId)
                throw new Exception("Invalid other user.");
        }

        private static void ReorderMessageImages(Message message)
        {
            var orderedImages = message.Images.OrderBy(x => x.SortOrder).ToList();

            for (int i = 0; i < orderedImages.Count; i++)
                orderedImages[i].SortOrder = i;
        }

        private static MessageResponse MapMessageResponse(Message message)
        {
            return new MessageResponse
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                Text = message.Text,
                Images = message.Images
                    .OrderBy(x => x.SortOrder)
                    .Select(x => new MessageImageResponse
                    {
                        Id = x.Id,
                        ImageUrl = x.ImageUrl,
                        SortOrder = x.SortOrder
                    })
                    .ToList(),
                FileUrl = message.FileUrl,
                FileName = message.FileName,
                FileSize = message.FileSize,
                FileContentType = message.FileContentType,
                SentAtUtc = message.SentAtUtc,
                IsRead = message.IsRead,
                IsEdited = message.IsEdited,
                EditedAtUtc = message.EditedAtUtc,
                IsDeleted = message.IsDeleted,
                DeletedAtUtc = message.DeletedAtUtc
            };
        }

        private static MessageType ResolveMessageType(string? text, bool hasImages, string? fileUrl)
        {
            var hasText = !string.IsNullOrWhiteSpace(text);
            var hasFile = !string.IsNullOrWhiteSpace(fileUrl);

            if (hasText && !hasImages && !hasFile) return MessageType.Text;
            if (!hasText && hasImages && !hasFile) return MessageType.Image;
            if (!hasText && !hasImages && hasFile) return MessageType.File;
            return MessageType.Mixed;
        }

        private static string BuildConversationLastMessage(Message message)
        {
            if (message.IsDeleted) return "[Deleted]";
            if (!string.IsNullOrWhiteSpace(message.Text)) return message.Text!;

            var hasImages = message.Images.Any();
            var hasFile = !string.IsNullOrWhiteSpace(message.FileUrl);

            if (hasImages && hasFile) return "[Images + File]";
            if (hasImages) return message.Images.Count > 1 ? "[Images]" : "[Image]";
            if (hasFile) return "[File]";

            return string.Empty;
        }
    }
}