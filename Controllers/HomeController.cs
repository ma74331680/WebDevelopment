using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using WebDevelopment.Data;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    public HomeController(ApplicationDbContext db) => _db = db;

    // GET /
    public async Task<IActionResult> Index()
    {
        var latest = await _db.Artworks
            .AsNoTracking()
            .OrderByDescending(a => a.UploadDate)
            .Take(12)                     // 首頁顯示 12 張
            .ToListAsync();

        return View(latest);
    }
}
