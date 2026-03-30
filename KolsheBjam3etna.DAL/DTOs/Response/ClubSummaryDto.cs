using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.DTOs.Response
{
    public class ClubSummaryDto
    {
        public int ActiveCount { get; set; }
        public int ExpiringSoonCount { get; set; }
        public int ExpiredCount { get; set; }
    }
}
