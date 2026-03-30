using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Portehobe.src.PorteHobe.API.DTOs;
using Portehobe.src.PorteHobe.Domain.Entities;
using Portehobe.src.PorteHobe.Infrastructure;

namespace Portehobe.src.PorteHobe.API.Services
{
    public class TaskItemService : ITaskItemService
    {
        private readonly AppDbContext _context;

        public TaskItemService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskItemDto>> GetUserTasksAsync(string userId)
        {
            // Traverse up the chain: Task -> Subject -> Term -> AppUser
            var tasks = await _context.TaskItems
                .Where(t => t.Subject.Term.AppUserId == userId)
                .OrderBy(t => t.DueDate)
                .ToListAsync();

            return tasks.Select(MapToDto);
        }

        public async Task<TaskItemDto?> GetTaskByIdAsync(int id, string userId)
        {
            var task = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == id && t.Subject.Term.AppUserId == userId);

            return task == null ? null : MapToDto(task);
        }

        public async Task<TaskItemDto> CreateTaskAsync(CreateTaskItemDto dto, string userId)
        {
            // SECURITY CHECK: Ensure the user actually owns the Subject they are trying to add a task to!
            var subjectBelongsToUser = await _context.Subjects
                .AnyAsync(s => s.Id == dto.SubjectId && s.Term.AppUserId == userId);

            if (!subjectBelongsToUser)
            {
                throw new UnauthorizedAccessException("You cannot add a task to a subject you do not own.");
            }

            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                Priority = dto.Priority,
                Status = TaskitemStatus.Pending, // Default for new tasks
                SubjectId = dto.SubjectId
            };

            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();

            return MapToDto(task);
        }

        public async Task<bool> UpdateTaskAsync(int id, UpdateTaskItemDto dto, string userId)
        {
            var task = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == id && t.Subject.Term.AppUserId == userId);

            if (task == null) return false;

            // Security check: if user are changing the subject, make sure they own the new one too
            if (task.SubjectId != dto.SubjectId)
            {
                var newSubjectBelongsToUser = await _context.Subjects
                    .AnyAsync(s => s.Id == dto.SubjectId && s.Term.AppUserId == userId);
                if (!newSubjectBelongsToUser) return false;
            }

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.DueDate = dto.DueDate;
            task.Priority = dto.Priority;
            task.Status = dto.Status;
            task.SubjectId = dto.SubjectId;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTaskAsync(int id, string userId)
        {
            var task = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == id && t.Subject.Term.AppUserId == userId);

            if (task == null) return false;

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        // Helper method to map Entity to DTO
        private static TaskItemDto MapToDto(TaskItem task)
        {
            return new TaskItemDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority.ToString(),
                Status = task.Status.ToString(),
                SubjectId = task.SubjectId
            };
        }
    }
}