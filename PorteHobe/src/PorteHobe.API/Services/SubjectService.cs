using Microsoft.EntityFrameworkCore;
using Portehobe.Model;
using Portehobe.src.PorteHobe.API.DTOs;
using Portehobe.src.PorteHobe.Infrastructure;

namespace PorteHobe.API.Services;

public class SubjectService : ISubjectService
{
    private readonly AppDbContext _context;

    public SubjectService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto dto)
    {
        // Check for duplicate code (if code is provided)
        if (!string.IsNullOrEmpty(dto.Code))
        {
            var exists = await _context.Subjects.AnyAsync(s => s.Code == dto.Code);
            if (exists) throw new InvalidOperationException($"Subject code '{dto.Code}' already exists.");
        }

        var subject = new Subject
        {
            Name = dto.Name,
            Code = dto.Code,
            Description = dto.Description
        };

        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        return MapToDto(subject);
    }

    public async Task<SubjectDto?> GetSubjectByIdAsync(int id)
    {
        var subject = await _context.Subjects.FindAsync(id);
        return subject == null ? null : MapToDto(subject);
    }

    public async Task<IEnumerable<SubjectDto>> GetAllSubjectsAsync(int pageNumber = 1, int pageSize = 10)
    {
        var subjects = await _context.Subjects
            .OrderByDescending(s => s.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return subjects.Select(MapToDto);
    }

    public async Task<SubjectDto?> UpdateSubjectAsync(int id, UpdateSubjectDto dto)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject == null) return null;

        // Check duplicate code on update
        if (!string.IsNullOrEmpty(dto.Code) && subject.Code != dto.Code)
        {
            var exists = await _context.Subjects.AnyAsync(s => s.Code == dto.Code);
            if (exists) throw new InvalidOperationException($"Subject code '{dto.Code}' already exists.");
        }

        subject.Name = dto.Name;
        subject.Code = dto.Code;
        subject.Description = dto.Description;

        await _context.SaveChangesAsync();
        return MapToDto(subject);
    }

    public async Task<bool> DeleteSubjectAsync(int id)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject == null) return false;

        _context.Subjects.Remove(subject);
        await _context.SaveChangesAsync();
        return true;
    }

    // Helper method to keep mapping clean
    private static SubjectDto MapToDto(Subject subject)
    {
        return new SubjectDto
        {
            Id = subject.Id,
            Name = subject.Name,
            Code = subject.Code,
            Description = subject.Description,
            CreatedAt = subject.CreatedAt,
            UpdatedAt = subject.UpdatedAt
        };
    }
}