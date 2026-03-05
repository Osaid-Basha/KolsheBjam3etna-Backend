using KolsheBjam3etna.DAL.Data;
using KolsheBjam3etna.DAL.Models;
using KolsheBjam3etna.DAL.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
namespace KolsheBjam3etna.DAL.Repository.Implementations
{
    public class OfferRepository : IOfferRepository
    {
        private readonly ApplicationDbContext _db;
        public OfferRepository(ApplicationDbContext db) => _db = db;

        public async Task<string?> ResolveReceiverIdAsync(OfferTargetType type, int targetId)
        {
            return type switch
            {
                OfferTargetType.ServiceRequest => await _db.ServiceRequests
                    .Where(x => x.Id == targetId).Select(x => x.UserId).FirstOrDefaultAsync(),

                OfferTargetType.ProductAd => await _db.ProductAds
                    .Where(x => x.Id == targetId).Select(x => x.UserId).FirstOrDefaultAsync(),

                OfferTargetType.SwapAd => await _db.SwapAds
                    .Where(x => x.Id == targetId).Select(x => x.UserId).FirstOrDefaultAsync(),

                _ => null
            };
        }

        public async Task<string?> ResolveTargetTitleAsync(OfferTargetType type, int targetId)
        {
            return type switch
            {
                OfferTargetType.ServiceRequest => await _db.ServiceRequests
                    .Where(x => x.Id == targetId).Select(x => x.Title).FirstOrDefaultAsync(),

                OfferTargetType.ProductAd => await _db.ProductAds
                    .Where(x => x.Id == targetId).Select(x => x.Title).FirstOrDefaultAsync(),

                OfferTargetType.SwapAd => await _db.SwapAds
                    .Where(x => x.Id == targetId).Select(x => x.OfferTitle).FirstOrDefaultAsync(),

                _ => null
            };
        }

        public async Task AddAsync(Offer offer) => await _db.Offers.AddAsync(offer);
        public async Task SaveAsync() => await _db.SaveChangesAsync();

        public Task<List<Offer>> GetIncomingAsync(string userId) =>
            _db.Offers.AsNoTracking()
                .Include(x => x.Sender)
                .Where(x => x.ReceiverId == userId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync();

        public Task<List<Offer>> GetOutgoingAsync(string userId) =>
            _db.Offers.AsNoTracking()
                .Include(x => x.Receiver)
                .Where(x => x.SenderId == userId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync();

        public Task<Offer?> GetByIdAsync(int id) =>
            _db.Offers.Include(x => x.Sender).Include(x => x.Receiver).FirstOrDefaultAsync(x => x.Id == id);
    }
}
