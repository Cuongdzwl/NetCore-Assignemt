using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetCore_Assignemt.Areas.Identity.Data;
using NetCore_Assignemt.Models;

namespace NetCore_Assignemt.Data
{
    public class AppDbContext : IdentityDbContext<NetCore_AssignemtUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<Book> Book { get; set; } = default!;
        public DbSet<Author> Author { get; set; } = default!;
    }
}