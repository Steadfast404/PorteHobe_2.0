using Microsoft.EntityFrameworkCore;
using Portehobe.Model;
using Portehobe.src.PorteHobe.API.DTOs;
using Portehobe.src.PorteHobe.Infrastructure;
using Portehobe.src.PorteHobe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PorteHobe.API.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly AppDbContext _context;

        public SubjectService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto dto, string userId)
        {
            // 1. Find the logged-in user's active term
            var activeTerm = await _context.Terms
                .Where(t => t.AppUserId == userId)
                .OrderByDescending(t => t.Id) // Grabs the newest one
                .FirstOrDefaultAsync();

            if (activeTerm == null)
            {
                throw new InvalidOperationException("You must have an active term to create a subject.");
            }

            var subject = new Subject
            {
                Name = dto.Name,
                Code = dto.Code,
                Description = dto.Description,
                TermId = activeTerm.Id 
            };

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return MapToDto(subject);
        }

        public async Task<IEnumerable<SubjectDto>> GetAllSubjectsAsync(int pageNumber, int pageSize, string userId)
        {
            var subjects = await _context.Subjects
                .Where(s => s.Term.AppUserId == userId) // SECURITY: Only get THEIR subjects
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return subjects.Select(MapToDto);
        }

        public async Task<SubjectDto?> GetSubjectByIdAsync(int id, string userId)
        {
            var subject = await _context.Subjects
                .FirstOrDefaultAsync(s => s.Id == id && s.Term.AppUserId == userId); // SECURITY CHECK

            return subject == null ? null : MapToDto(subject);
        }

        public async Task<SubjectDto?> UpdateSubjectAsync(int id, UpdateSubjectDto dto, string userId)
        {
            var subject = await _context.Subjects
                .FirstOrDefaultAsync(s => s.Id == id && s.Term.AppUserId == userId); // SECURITY CHECK

            if (subject == null) return null;

            subject.Name = dto.Name;
            subject.Code = dto.Code;
            subject.Description = dto.Description;

            await _context.SaveChangesAsync();
            return MapToDto(subject);
        }

        public async Task<bool> DeleteSubjectAsync(int id, string userId)
        {
            var subject = await _context.Subjects
                .FirstOrDefaultAsync(s => s.Id == id && s.Term.AppUserId == userId); // SECURITY CHECK

            if (subject == null) return false;

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
            return true;
        }

        // Keep your existing MapToDto helper method down here...
        private static SubjectDto MapToDto(Subject subject)
        {
            return new SubjectDto
            {
                Id = subject.Id,
                Name = subject.Name,
                Code = subject.Code,
                Description = subject.Description,
                //TermId = subject.TermId
            };
        }
    }
}