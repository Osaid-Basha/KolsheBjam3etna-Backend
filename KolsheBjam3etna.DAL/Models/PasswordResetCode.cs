using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Models
{
    public class PasswordResetCode
    {
        public int Id { get; set; }
        public string UserId { get; set; } = default!;
        public string CodeHash { get; set; } = default!;
        public DateTime ExpiresAtUtc { get; set; }
        public bool Used { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public ApplicationUser User { get; set; } = default!;
    }
}
