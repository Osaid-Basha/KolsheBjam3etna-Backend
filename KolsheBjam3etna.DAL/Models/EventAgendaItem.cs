using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Models
{
    public class EventAgendaItem
    {
        public int Id { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; } = default!;

        public string Title { get; set; } = "";       
        public TimeSpan? StartTime { get; set; }      
        public int Order { get; set; }              
        public bool IsVisible { get; set; } = true;   
    }
}
