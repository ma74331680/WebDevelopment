using Microsoft.AspNetCore.Mvc;
using WebDevelopment.Data;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    public HomeController(ApplicationDbContext db) => _db = db;

    // GET /
    public async Task<IActionResult> Index()
    {
        return View();
    }

}
