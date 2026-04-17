using KolsheBjam3etna.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KolsheBjam3etna.DAL.Utils
{
    public class UserSeedData : ISeedData
    {
        public int Order => 2;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserSeedData(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task Seed()
        {
            await SeedUserAsync(
                fullName: "osayd albasha",
                email: "albashaosayd@gmail.com",
                password: "Password123!",
                role: "SuperAdmin"
            );

            await SeedUserAsync(
                fullName: "ameer albasha",
                email: "ameeralbasha@gmail.com",
                password: "Password123!",
                role: "Coordinator"
            );

            await SeedUserAsync(
                fullName: "anas albasha",
                email: "albashaanas@gmail.com",
                password: "Password123!",
                role: "User"
            );
        }

        private async Task SeedUserAsync(string fullName, string email, string password, string role)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    FullName = fullName,
                    UserName = email,
                    Email = email
                };

                var createResult = await _userManager.CreateAsync(user, password);

                if (!createResult.Succeeded)
                {
                    throw new Exception($"Failed to create user {email}: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                }
            }

            if (!await _userManager.IsInRoleAsync(user, role))
            {
                var addToRoleResult = await _userManager.AddToRoleAsync(user, role);

                if (!addToRoleResult.Succeeded)
                {
                    throw new Exception($"Failed to assign role {role} to {email}: {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}