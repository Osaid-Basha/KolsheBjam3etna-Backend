    using KolsheBjam3etna.DAL.Models;
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    namespace KolsheBjam3etna.DAL.Utils
    {
    public class UserSeedData : ISeedData
    {
        public int Order => 2;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserSeedData(UserManager<ApplicationUser> userManager) => _userManager = userManager;

        public async Task Seed()
        {
            if (await _userManager.Users.AnyAsync()) return;

            var user1 = new ApplicationUser { FullName = "osayd albasha", UserName = "albashaosayd@gmail.com", Email = "albashaosayd@gmail.com" };
            var user2 = new ApplicationUser { FullName = "ameer albasha", UserName = "ameeralbasha@gmail.com", Email = "ameeralbasha@gmail.com" };
            var user3 = new ApplicationUser { FullName = "anas albasha", UserName = "albashaanas@gmail.com", Email = "albashaanas@gmail.com" };

            await _userManager.CreateAsync(user1, "Password123!");
            await _userManager.CreateAsync(user2, "Password123!");
            await _userManager.CreateAsync(user3, "Password123!");

            await _userManager.AddToRoleAsync(user1, "Admin");
            await _userManager.AddToRoleAsync(user2, "Coordinator");
            await _userManager.AddToRoleAsync(user3, "User");
        }
    }
}
