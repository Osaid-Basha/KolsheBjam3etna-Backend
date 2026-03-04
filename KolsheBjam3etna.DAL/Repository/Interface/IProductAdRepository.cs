using KolsheBjam3etna.DAL.DTOs.Response;
using KolsheBjam3etna.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Repository.Interface
{
    public interface IProductAdRepository
    {
        Task<bool> CategoryExistsAsync(int id);

        Task AddAsync(ProductAd ad);

        Task SaveAsync();
       
    }
}
