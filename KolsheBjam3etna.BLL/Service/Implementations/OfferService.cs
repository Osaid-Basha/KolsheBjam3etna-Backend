using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using KolsheBjam3etna.DAL.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.BLL.Service.Implementations
{
    public class OfferService : IOfferService
    {
        private readonly IOfferRepository _repo;
        private readonly INotificationService _notificationService;     

        public OfferService(IOfferRepository repo,INotificationService notificationService)
        {
            _repo = repo;
            _notificationService = notificationService;
        }

        private static bool TryParseTarget(string s, out OfferTargetType type)
        {
            type = default;
            if (string.IsNullOrWhiteSpace(s)) return false;

            return Enum.TryParse<OfferTargetType>(s.Trim(), ignoreCase: true, out type);
        }

        public async Task<ApiResponse<int>> CreateAsync(string senderId, CreateOfferRequest req)
        {
            if (!TryParseTarget(req.TargetType, out var type))
                return ApiResponse<int>.Fail("Invalid TargetType");

            if (req.TargetId <= 0)
                return ApiResponse<int>.Fail("TargetId is required");

            if (req.Price <= 0)
                return ApiResponse<int>.Fail("Price must be > 0");

            if (string.IsNullOrWhiteSpace(req.Availability))
                return ApiResponse<int>.Fail("Availability is required");

            var receiverId = await _repo.ResolveReceiverIdAsync(type, req.TargetId);
            if (string.IsNullOrEmpty(receiverId))
                return ApiResponse<int>.Fail("Target not found");

            if (receiverId == senderId)
                return ApiResponse<int>.Fail("You can't offer on your own post");

            var offer = new Offer
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                TargetType = type,
                TargetId = req.TargetId,
                Price = req.Price,
                Availability = req.Availability.Trim(),
                Message = string.IsNullOrWhiteSpace(req.Message) ? null : req.Message.Trim(),
                Status = OfferStatus.Pending
            };

            await _repo.AddAsync(offer);
            await _repo.SaveAsync();
            await _notificationService.CreateAsync(
       receiverId,
       "عرض جديد على طلبك",
       $"تم تقديم عرض على طلبك بسعر {req.Price} د./ساعة",
       "Offer",
       targetType: offer.TargetType.ToString(), 
       targetId: offer.TargetId                
   );

            return ApiResponse<int>.Ok(offer.Id, "Offer sent");
        }

        public async Task<ApiResponse<List<OfferCardDto>>> GetIncomingAsync(string userId)
        {
            var offers = await _repo.GetIncomingAsync(userId);

            var list = new List<OfferCardDto>();
            foreach (var o in offers)
            {
                var title = await _repo.ResolveTargetTitleAsync(o.TargetType, o.TargetId) ?? "";

                list.Add(new OfferCardDto
                {
                    Id = o.Id,
                    TargetType = o.TargetType.ToString(),
                    TargetId = o.TargetId,
                    TargetTitle = title,
                    Price = o.Price,
                    Availability = o.Availability,
                    Message = o.Message,
                    Status = o.Status.ToString(),
                    CreatedAtUtc = o.CreatedAtUtc,

                    OtherUserId = o.SenderId,
                    OtherUserName = o.Sender.FullName,
                    OtherUserProfileImageUrl = o.Sender.ProfileImageUrl
                });
            }

            return ApiResponse<List<OfferCardDto>>.Ok(list, "Success");
        }

        public async Task<ApiResponse<List<OfferCardDto>>> GetOutgoingAsync(string userId)
        {
            var offers = await _repo.GetOutgoingAsync(userId);

            var list = new List<OfferCardDto>();
            foreach (var o in offers)
            {
                var title = await _repo.ResolveTargetTitleAsync(o.TargetType, o.TargetId) ?? "";

                list.Add(new OfferCardDto
                {
                    Id = o.Id,
                    TargetType = o.TargetType.ToString(),
                    TargetId = o.TargetId,
                    TargetTitle = title,
                    Price = o.Price,
                    Availability = o.Availability,
                    Message = o.Message,
                    Status = o.Status.ToString(),
                    CreatedAtUtc = o.CreatedAtUtc,

                    OtherUserId = o.ReceiverId,
                    OtherUserName = o.Receiver.FullName,
                    OtherUserProfileImageUrl = o.Receiver.ProfileImageUrl
                });
            }

            return ApiResponse<List<OfferCardDto>>.Ok(list, "Success");
        }

        public async Task<ApiResponse<string>> AcceptAsync(string receiverId, int offerId)
        {
            var offer = await _repo.GetByIdAsync(offerId);
            if (offer == null) return ApiResponse<string>.Fail("Offer not found");

            if (offer.ReceiverId != receiverId)
                return ApiResponse<string>.Fail("Not allowed");

            if (offer.Status != OfferStatus.Pending)
                return ApiResponse<string>.Fail("Offer already responded");

            offer.Status = OfferStatus.Accepted;
            offer.RespondedAtUtc = DateTime.UtcNow;

            await _repo.SaveAsync();
            await _notificationService.CreateAsync(
    offer.SenderId,
    "تم قبول عرضك",
    "تم قبول عرضك ✅ يمكنك التواصل الآن",
    "Offer",
    targetType: "Offer",
    targetId: offer.Id
);
            return ApiResponse<string>.Ok("Accepted", "Offer accepted");
        }

        public async Task<ApiResponse<string>> RejectAsync(string receiverId, int offerId)
        {
            var offer = await _repo.GetByIdAsync(offerId);
            if (offer == null) return ApiResponse<string>.Fail("Offer not found");

            if (offer.ReceiverId != receiverId)
                return ApiResponse<string>.Fail("Not allowed");

            if (offer.Status != OfferStatus.Pending)
                return ApiResponse<string>.Fail("Offer already responded");

            offer.Status = OfferStatus.Rejected;
            offer.RespondedAtUtc = DateTime.UtcNow;

            await _repo.SaveAsync();
            await _notificationService.CreateAsync(
    offer.SenderId,
    "تم رفض عرضك",
    "للأسف تم رفض عرضك",
    "Offer",
    targetType: "Offer",
    targetId: offer.Id
);
            return ApiResponse<string>.Ok("Rejected", "Offer rejected");
        }
    }
}
