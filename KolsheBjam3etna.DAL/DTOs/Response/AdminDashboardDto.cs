using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Response
{
    public class AdminDashboardDto
    {
        public int AdsCount { get; set; }
        public int UsersCount { get; set; }
        public int MessagesCount { get; set; }
        public int ReportsCount { get; set; } 

        public List<DailyPointDto> Activity7Days { get; set; } = new();
    }

    public class DailyPointDto
    {
        public string Day { get; set; } = ""; 
        public int Value { get; set; }
    }
}
