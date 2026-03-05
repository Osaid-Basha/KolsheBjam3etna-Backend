using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.BLL.Service.Interface
{
    public interface IOfferService
    {
        Task<ApiResponse<int>> CreateAsync(string senderId, CreateOfferRequest req);
        Task<ApiResponse<List<OfferCardDto>>> GetIncomingAsync(string userId);
        Task<ApiResponse<List<OfferCardDto>>> GetOutgoingAsync(string userId);

        Task<ApiResponse<string>> AcceptAsync(string receiverId, int offerId);
        Task<ApiResponse<string>> RejectAsync(string receiverId, int offerId);
    }
}
