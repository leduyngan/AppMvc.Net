using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Data;
using App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.Database.Controllers
{
    [Area("Database")]
    [Route("/database-manage/[action]")]
    public class DbManageController : Controller
    {
        private readonly AppDbContext _dbConetxt;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbManageController(AppDbContext dbConetxt, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dbConetxt = dbConetxt;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult DeleteDb()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteDbAsnyc()
        {
            var success = await _dbConetxt.Database.EnsureDeletedAsync();
            StatusMessage = success ? "Xóa database thành công" : "Không xóa được Db";

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Migrate()
        {
            await _dbConetxt.Database.MigrateAsync();
            StatusMessage = "Cập nhật database thành công";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SeedDataAsync()
        {
            var roleNames = typeof(RoleName).GetFields().ToList();
            foreach (var r in roleNames)
            {
                var roleName = (string)r.GetRawConstantValue();
                var rFound = await _roleManager.FindByNameAsync(roleName);
                if (rFound == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            var userAdmin = await _userManager.FindByNameAsync("admin");
            if (userAdmin == null)
            {
                userAdmin = new AppUser()
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true,
                };
                await _userManager.CreateAsync(userAdmin, "admin123");
                await _userManager.AddToRoleAsync(userAdmin, RoleName.Administrator);

            }
            StatusMessage = "Vừa seed database";
            return RedirectToAction("Index");
        }

    }
}