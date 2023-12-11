using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetCore_Assignemt.Areas.Identity.Data;
using NetCore_Assignemt.Models;

namespace NetCore_Assignemt.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Các đoạn mã cấu hình khác của bạn

            SeedRoles(modelBuilder);
            SeedUsers(modelBuilder);
        }

        private void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "User", NormalizedName = "USER" },
                new IdentityRole { Id = "2", Name = "Admin", NormalizedName = "ADMIN" }
            );
        }

        private void SeedUsers(ModelBuilder modelBuilder)
        {
            // Chú ý: Trong trường hợp thực tế, bạn có thể cần tạo password hash thay vì lưu trực tiếp password
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = "1",
                    UserName = "admin@gmail.com",
                    NormalizedUserName = "ADMIN@GMAIL.COM",
                    Email = "admin@gmail.com",
                    NormalizedEmail = "ADMIN@GMAIL.COM",
                    EmailConfirmed = true,
                    PasswordHash = new PasswordHasher<User>().HashPassword(null, "P@ssw0rd"),
                    SecurityStamp = string.Empty
                },
                new User
                {
                    Id = "2",
                    UserName = "test1@gmail.com",
                    NormalizedUserName = "TEST1@GMAIL.COM",
                    Email = "test1@gmail.com",
                    NormalizedEmail = "TEST1@GMAIL.COM",
                    EmailConfirmed = true,
                    PasswordHash = new PasswordHasher<User>().HashPassword(null, "P@ssw0rd"),
                    SecurityStamp = string.Empty
                }
            );

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = "1", RoleId = "1" }, // User role
                new IdentityUserRole<string> { UserId = "2", RoleId = "2" }  // Admin role
            );
        }
        public DbSet<Book> Book { get; set; } = default!;
        public DbSet<Author> Author { get; set; } = default!; 
        public DbSet<BookAuthor> BookAuthor { get; set; } = default!;
        public DbSet<BookCategory> BookCategory { get; set; } = default!;
        public DbSet<Category> Category { get; set; } = default!;
        public DbSet<Cart> Cart { get; set; } = default!;
        public DbSet<Order> Order { get; set; } = default!;
        public DbSet<OrderDetail> OrderDetail { get; set; } = default!;

        
    }
}