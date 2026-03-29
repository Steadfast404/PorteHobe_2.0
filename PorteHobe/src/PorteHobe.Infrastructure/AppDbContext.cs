using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Portehobe.Model;
using Portehobe.src.PorteHobe.Domain.Entities;
using PorteHobe.Domain.Entities;

namespace Portehobe.src.PorteHobe.Infrastructure
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Term> Terms { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<StudySession> StudySessions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "1" },
                new IdentityRole { Id = "2", Name = "Student", NormalizedName = "STUDENT", ConcurrencyStamp = "2" }
            );

            builder.Entity<StudySession>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.HasOne(s => s.AppUser)
                      .WithMany(u => u.StudySessions)
                      .HasForeignKey(s => s.AppUserId)
                      .OnDelete(DeleteBehavior.Cascade);   // deleting user deletes their sessions

                entity.HasOne(s => s.Subject)
                      .WithMany(sub => sub.StudySessions)
                      .HasForeignKey(s => s.SubjectId)
                      .OnDelete(DeleteBehavior.Restrict);  // or DeleteBehavior.NoAction

                entity.HasOne(s => s.TaskItem)
                      .WithMany(t => t.StudySessions)
                      .HasForeignKey(s => s.TaskItemId)
                      .OnDelete(DeleteBehavior.Restrict);  // <— change from SetNull to Restrict/NoAction
            });
        }
    }
}