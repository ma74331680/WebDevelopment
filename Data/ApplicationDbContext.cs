using Microsoft.EntityFrameworkCore;
using WebDevelopment.Models;

namespace WebDevelopment.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Artwork> Artworks { get; set; }
    }
}
