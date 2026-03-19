using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portehobe.Model;
using PorteHobe.Domain.Entities;

namespace PorteHobe.Infrastructure.Data.Configurations;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        // Name is required and has a max length
        builder.Property(s => s.Name)
               .IsRequired()
               .HasMaxLength(100);

        // Code has a max length and must be unique
        builder.Property(s => s.Code)
               .HasMaxLength(20);

        builder.HasIndex(s => s.Code)
               .IsUnique();

        // Description max length
        builder.Property(s => s.Description)
               .HasMaxLength(500);
    }
}