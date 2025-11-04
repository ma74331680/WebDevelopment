using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebDevelopment.Data;
using WebDevelopment.Models;
using WebDevelopment.ViewModels;

[Authorize] // 需登入才可上傳
public class ArtworkController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IWebHostEnvironment _env;

    public ArtworkController(ApplicationDbContext db, UserManager<IdentityUser> um, IWebHostEnvironment env)
    {
        _db = db;
        _userManager = um;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var latest = await _db.Artworks
            .AsNoTracking()
            .OrderByDescending(a => a.UploadDate)
            .Take(12)                     // 首頁顯示 12 張
            .ToListAsync();

        return View(latest);
    }

    // GET: /Artwork/Create
    public IActionResult Upload() => View(new ArtworkCreateVm());

    // POST: /Artwork/Create
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(ArtworkCreateVm vm, IFormFile image)
    {
        // 1) 基本表單驗證
        if (image == null || image.Length == 0)
            ModelState.AddModelError(nameof(image), "請選擇要上傳的圖片。");

        var allowed = new[] { "image/png", "image/jpeg", "image/webp" };
        if (image != null && !allowed.Contains(image.ContentType))
            ModelState.AddModelError(nameof(image), "僅支援 PNG / JPEG / WEBP。");

        if (image != null && image.Length > 10 * 1024 * 1024)
            ModelState.AddModelError(nameof(image), "檔案過大，請小於 10 MB。");

        if (!ModelState.IsValid) return View(vm);

        // 2) 產生存檔路徑（/wwwroot/uploads/yyyy/MM/uuid.ext）
        var y = DateTime.UtcNow.Year;
        var m = DateTime.UtcNow.Month.ToString("D2");
        var relDir = Path.Combine("uploads", y.ToString(), m);
        var absDir = Path.Combine(_env.WebRootPath, relDir);
        Directory.CreateDirectory(absDir);

        var ext = Path.GetExtension(image.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}{ext}";
        var absPath = Path.Combine(absDir, fileName);
        await using (var fs = System.IO.File.Create(absPath))
            await image.CopyToAsync(fs);

        // 3) 建立資料列
        var artwork = new Artwork
        {
            Title = vm.Title,
            Description = vm.Description,
            ImageUrl = "/" + Path.Combine(relDir, fileName).Replace("\\", "/"),
            ArtistName = _userManager.GetUserId(User) ?? string.Empty,
            UploadDate = DateTime.UtcNow
        };

        _db.Artworks.Add(artwork);
        await _db.SaveChangesAsync();

        // 4) 轉到作品詳細頁
        return RedirectToAction(nameof(Details), new { id = artwork.Id });
    }

    // 我的作品清單
    public async Task<IActionResult> My()
    {
        var uid = _userManager.GetUserId(User) ?? string.Empty;
        var mine = await _db.Artworks
            .Where(a => a.ArtistName == uid)
            .OrderByDescending(a => a.UploadDate)
            .ToListAsync();
        return View(mine);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var model = await _db.Artworks.FirstOrDefaultAsync(a => a.Id == id);
        if (model == null) return NotFound();

        // 查作者的 UserName
        var user = await _userManager.FindByIdAsync(model.ArtistName);
        ViewBag.ArtistName = user?.UserName ?? "未知作者";

        return View(model);
    }
}
