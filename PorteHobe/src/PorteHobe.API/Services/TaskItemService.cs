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
            var tasks = await _context.TaskItems
                .Where(t => t.UserId == userId)
                .OrderBy(t => t.DueDate)
                .ToListAsync();

            return tasks.Select(MapToDto);
        }

        public async Task<TaskItemDto?> GetTaskByIdAsync(int id, string userId)
        {
            var task = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            return task == null ? null : MapToDto(task);
        }

        public async Task<TaskItemDto> CreateTaskAsync(CreateTaskItemDto dto, string userId)
        {
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                Priority = dto.Priority,
                Status = TaskitemStatus.Pending, // Default for new tasks
                UserId = userId,
                SubjectId = dto.SubjectId
            };

            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();

            return MapToDto(task);
        }

        public async Task<bool> UpdateTaskAsync(int id, UpdateTaskItemDto dto, string userId)
        {
            var task = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task == null) return false;

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
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

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
