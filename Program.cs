using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebDevelopment.Data;

var builder = WebApplication.CreateBuilder(args);

// 加入 MVC 與資料庫設定
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 使用預設 Identity（需要登入/註冊）
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // 有寄信驗證時改 true
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages(); // 內建 Identity UI 用 Razor Pages

var app = builder.Build();

// 基本中介軟體設定
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Identity UI 需要
app.MapRazorPages();

await SeedData.EnsureSeedAsync(app.Services);

app.Run();
