using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Request
{
    public class AgendaItemRequest
    {
        public string Title { get; set; } = "";
        public string? StartTime { get; set; } 
        public int Order { get; set; }
        public bool IsVisible { get; set; } = true;
    }
}
