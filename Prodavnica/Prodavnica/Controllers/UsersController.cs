﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Prodavnica.Models;

namespace Prodavnica.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("/Admin/[controller]/{action=Index}/{id?}")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly int pageSize = 3;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public IActionResult Index(int? pageIndex)
        {
            IQueryable<ApplicationUser> query = userManager.Users.OrderByDescending(u => u.CreatedAt);

            //paginacija
            if (pageIndex == null || pageIndex < 1)
            {
                pageIndex = 1;
            }

            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip(((int)pageIndex - 1) * pageSize).Take(pageSize);

            var users = query.ToList();

            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalPages = totalPages;

            return View(users);
        }

        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var appUser = await userManager.FindByIdAsync(id);

            if (appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Roles = await userManager.GetRolesAsync(appUser);

            //dostupne uloge
            var availableRoles = roleManager.Roles.ToList();
            var items = new List<SelectListItem>();
            foreach (var role in availableRoles)
            {
                items.Add(
                    new SelectListItem
                    {
                        Text = role.NormalizedName,
                        Value = role.Name,
                        Selected = await userManager.IsInRoleAsync(appUser, role.Name!),
                    });
            }

            ViewBag.SelectItems = items;

            return View(appUser);
        }


        public async Task<IActionResult> EditRole(string? id, string? newRole) 
        {
            if (id == null || newRole == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var roleExists = await roleManager.RoleExistsAsync(newRole);
            var appUser = await userManager.FindByIdAsync(id);

            if (appUser == null || !roleExists) 
            {
                return RedirectToAction("Index", "Home");
            }

            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser!.Id == appUser.Id)
            {
                TempData["ErrorMessage"] = "You can not update your own role";
                return RedirectToAction("Details", "Users", new { id });
            }

            //promena uloge
            var userRoles = await userManager.GetRolesAsync(appUser);
            await userManager.RemoveFromRolesAsync(appUser, userRoles);
            await userManager.AddToRoleAsync(appUser, newRole);

            TempData["SuccessMessage"] = "User role updated successfully";
            return RedirectToAction("Details", "Users", new { id });
        }

        public async Task<IActionResult> DeleteAccount(string? id) 
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var appUser = await userManager.FindByIdAsync(id);

            if (appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser!.Id == appUser.Id)
            {
                TempData["ErrorMessage"] = "You can not delete your own account";
                return RedirectToAction("Details", "Users", new { id });
            }

            //brisanje akaunta
            var result = await userManager.DeleteAsync(appUser);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Users");
            }

            TempData["ErrorMessage"] = "Unable to delete this account: " + result.Errors.First().Description;
            return RedirectToAction("Details", "Users", new { id });
        }
    }
}
