using KolsheBjam3etna.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Repository.Interface
{
    public interface IOfferRepository
    {
        Task<string?> ResolveReceiverIdAsync(OfferTargetType type, int targetId);
        Task<string?> ResolveTargetTitleAsync(OfferTargetType type, int targetId);

        Task AddAsync(Offer offer);
        Task SaveAsync();

        Task<List<Offer>> GetIncomingAsync(string userId);
        Task<List<Offer>> GetOutgoingAsync(string userId);

        Task<Offer?> GetByIdAsync(int id);
    }
}
