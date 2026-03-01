using Microsoft.EntityFrameworkCore;
using PorteHobe.Domain.Entities;

namespace Portehobe.Model
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Resource> Resources { get; set; }
    }
}