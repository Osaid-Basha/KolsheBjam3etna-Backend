using KolsheBjam3etna.DAL.Data;
using KolsheBjam3etna.DAL.Models;
using KolsheBjam3etna.DAL.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace KolsheBjam3etna.DAL.Repository.Implementations
{
    public class SwapAdRepository : ISwapAdRepository
    {
        private readonly ApplicationDbContext _db;

        public SwapAdRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<bool> CategoryExistsAsync(int id)
        {
            return _db.ProductCategories.AnyAsync(x => x.Id == id);
        }

        public async Task AddAsync(SwapAd ad)
        {
            await _db.SwapAds.AddAsync(ad);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
