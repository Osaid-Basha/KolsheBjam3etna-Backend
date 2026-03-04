using KolsheBjam3etna.DAL.Data;
using KolsheBjam3etna.DAL.Models;
using KolsheBjam3etna.DAL.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace KolsheBjam3etna.DAL.Repository.Implementations
{
    public class ProductAdRepository : IProductAdRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductAdRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<bool> CategoryExistsAsync(int id)
        {
            return _db.ProductCategories.AnyAsync(x => x.Id == id);
        }

        public async Task AddAsync(ProductAd ad)
        {
            await _db.ProductAds.AddAsync(ad);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

    }
}
