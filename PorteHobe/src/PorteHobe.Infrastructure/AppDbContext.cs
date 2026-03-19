using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Portehobe.Model;
using PorteHobe.Domain.Entities;  // For Resource

namespace Portehobe.src.PorteHobe.Infrastructure  // AppUser and Subject are already in this namespace
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Term> Terms { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "1" },
                new IdentityRole { Id = "2", Name = "Student", NormalizedName = "STUDENT", ConcurrencyStamp = "2" }
            );
        }
    }
}