using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace KolsheBjam3etna.DAL.Utils
{
    public class RoleSeedData : ISeedData
    {
        public int Order => 1;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleSeedData(RoleManager<IdentityRole> roleManager) => _roleManager = roleManager;

        public async Task Seed()
        {
            string[] roles = { "Admin", "Coordinator", "User", "SuperAdmin" };

            foreach (var role in roles)
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}
