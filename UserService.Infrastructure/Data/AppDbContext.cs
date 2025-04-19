using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet<User> veritabanındaki "Users" tablosuna karşılık gelir
        public DbSet<User> Users { get; set; }
    }
}
