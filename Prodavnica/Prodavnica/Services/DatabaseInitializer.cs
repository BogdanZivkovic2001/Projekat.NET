using Microsoft.AspNetCore.Identity;
using Prodavnica.Models;

namespace Prodavnica.Services
{
    public class DatabaseInitializer
    {
        public static async Task SeedDataAsync(UserManager<ApplicationUser>? userManager,
            RoleManager<IdentityRole>? roleManager)
        {
            if (userManager == null || roleManager == null)
            {
                Console.WriteLine("userMenager or roleMenager is null => exit");
                return;
            }

            //proveravamo da li imamo ulogu admina ili ne
            var exists = await roleManager.RoleExistsAsync("admin");
            if (!exists)
            {
                Console.WriteLine("Admin role is not defined and it will be cretared");
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }

            //proveravamo da li imamo ulogu prodavca ili ne
            exists = await roleManager.RoleExistsAsync("seller");
            if (!exists)
            {
                Console.WriteLine("Seller role is not defined and it will be cretared");
                await roleManager.CreateAsync(new IdentityRole("seller"));
            }

            //proveravamo da li imamo ulogu korisnika ili ne
            exists = await roleManager.RoleExistsAsync("client");
            if (!exists)
            {
                Console.WriteLine("Client role is not defined and it will be cretared");
                await roleManager.CreateAsync(new IdentityRole("client"));
            }

            //proveravamo da li imamo makar jednog admina
            var adminUsers = await userManager.GetUsersInRoleAsync("admin");
            if (adminUsers.Any())
            {
                //Ako admin postoji => exit
                Console.WriteLine("Admin user already exists => exit");
                return;
            }

            //kreiranje admina
            var user = new ApplicationUser()
            {
                FirstName = "Admin",
                LastName = "Admin",
                UserName = "admin@admin.com",//username ce biti korisceno za autentifijaciju
                Email = "admin@admin.com",
                CreatedAt = DateTime.Now,
            };

            string initialPassword = "admin123";

            var result = await userManager.CreateAsync(user, initialPassword);
            if (result.Succeeded)
            {
                //uloga korisnika
                await userManager.AddToRoleAsync(user, "admin");
                Console.WriteLine("Admin user created secessfully! Please update the initial password!");
                Console.WriteLine("Email: " + user.Email);
                Console.WriteLine("Initial password: " + initialPassword);
            }
        }
    }
}
