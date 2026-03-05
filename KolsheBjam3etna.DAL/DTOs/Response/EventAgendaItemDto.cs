using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Response
{
    public class EventAgendaItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? StartTime { get; set; } 
        public int Order { get; set; }
        public bool IsVisible { get; set; }
    }
}
