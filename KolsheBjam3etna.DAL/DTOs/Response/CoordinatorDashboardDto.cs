using KolsheBjam3etna.DAL.DTOs.Response.KolsheBjam3etna.DAL.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Response
{
    public class CoordinatorDashboardDto
    {
        public int ActiveEventsCount { get; set; }
        public int TotalRegistrations { get; set; }
        public int RequestsCount { get; set; } 

        public double RegistrationRatePercent { get; set; } 
        public double AverageRating { get; set; }           

        public List<CoordinatorEventCardDto> Performance { get; set; } = new();
    }
}
