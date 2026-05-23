using Microsoft.EntityFrameworkCore;
using Lotomoto.Models;

namespace Lotomoto.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<CarListing> CarListings { get; set; }
    }
}