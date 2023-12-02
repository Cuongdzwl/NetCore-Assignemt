using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetCore_Assignemt.Models;

namespace NetCore_Assignemt.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<Book> Book { get; set; } = default!;
        public DbSet<Author> Author { get; set; } = default!;
        public DbSet<Category> Category { get; set; } = default!;
        public DbSet<Cart> Cart { get; set; } = default!;
        public DbSet<NetCore_Assignemt.Models.Order> Order { get; set; } = default!;
    }
}