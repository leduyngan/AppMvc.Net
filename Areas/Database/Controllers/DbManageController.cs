using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.Database.Controllers
{
    [Area("Database")]
    [Route("/database-manage/[action]")]
    public class DbManageController : Controller
    {
        private readonly AppDbContext _dbConetxt;

        public DbManageController(AppDbContext dbConetxt)
        {
            _dbConetxt = dbConetxt;
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
    }
}