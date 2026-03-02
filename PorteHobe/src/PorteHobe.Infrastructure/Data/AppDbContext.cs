using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Portehobe.Model
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Subject> Subjects { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed roles with fixed IDs to prevent non-deterministic model changes
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "1" },
                new IdentityRole { Id = "2", Name = "Student", NormalizedName = "STUDENT", ConcurrencyStamp = "2" }
            );
        }
    }
}