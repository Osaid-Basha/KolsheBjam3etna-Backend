using KolsheBjam3etna.DAL.Data;
using KolsheBjam3etna.DAL.Models;
using KolsheBjam3etna.DAL.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace KolsheBjam3etna.DAL.Repository.Implementations
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        private readonly ApplicationDbContext _db;

        public ServiceRequestRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<bool> CategoryExistsAsync(int categoryId)
            => _db.ServiceCategories.AnyAsync(x => x.Id == categoryId);

        public async Task AddAsync(ServiceRequest request)
            => await _db.ServiceRequests.AddAsync(request);

        public Task SaveChangesAsync()
            => _db.SaveChangesAsync();

    }
}
