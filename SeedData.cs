using Microsoft.EntityFrameworkCore;
using WebDevelopment.Data;
using WebDevelopment.Models;

public static class SeedData
{
    public static async Task EnsureSeedAsync(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();

        if (!await db.Artworks.AnyAsync())
        {
            db.Artworks.Add(new Artwork
            {
                Title = "範例作品 A",
                Description = "Demo",
                ImageUrl = "/uploads/demo-a.jpg",   // 你可以先準備幾張圖放 /wwwroot/uploads/
                ArtistName = "seed",
                UploadDate = DateTime.UtcNow
            });
            db.Artworks.Add(new Artwork
            {
                Title = "範例作品 B",
                Description = "Demo",
                ImageUrl = "/uploads/demo-b.jpg",
                ArtistName = "seed",
                UploadDate = DateTime.UtcNow.AddMinutes(-10)
            });
            await db.SaveChangesAsync();
        }
    }
}
