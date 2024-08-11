using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Prodavnica.Models;
using System.Runtime.InteropServices;

namespace Prodavnica.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return View(registerDto);
            }

            //kreiranje novog akaunta
            var user = new ApplicationUser()
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.Email,//User name ce biti korisceno za autentifikaciju korisnika
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                Address = registerDto.Address,
                CreatedAt = DateTime.Now,
            };

            var result = await userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                //uspesna registracija korinika
                await userManager.AddToRoleAsync(user, "client");

                //prijavljivanje korisnika
                await signInManager.SignInAsync(user, false);

                return RedirectToAction("Index", "Home");
            }

            //prikazivanje greski u slucaju neuspesne prijave
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(registerDto);
        }
    }
}
