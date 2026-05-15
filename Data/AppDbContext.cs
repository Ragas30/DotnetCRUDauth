using Microsoft.EntityFrameworkCore;
using DotnetCRUD.Models;

namespace DotnetCRUD.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products => Set<Product>();
    }
}