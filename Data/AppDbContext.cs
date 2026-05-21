using Microsoft.EntityFrameworkCore;
using DotnetCRUD.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DotnetCRUD.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var userRoleConverter = new EnumToStringConverter<UserRole>();

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion(userRoleConverter)
                .HasMaxLength(20);
        }
    }
};
