using KolsheBjam3etna.DAL.Data;
using KolsheBjam3etna.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Utils
{
    public class UniversitySeedData : ISeedData
    {
        private readonly ApplicationDbContext _context;

        public UniversitySeedData(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Seed()
        {
            if (!_context.Universities.Any())
            {
                var universities = new List<University>
            {
                new University { Name = "الجامعة النجاح" },
                new University { Name = "جامعة العلوم والتكنولوجيا" },
                new University { Name = "الجامعة بيرزيت" },
                new University { Name = "جامعة خضوري" }
            };

                await _context.Universities.AddRangeAsync(universities);
                await _context.SaveChangesAsync();
            }
        }
    }
}
