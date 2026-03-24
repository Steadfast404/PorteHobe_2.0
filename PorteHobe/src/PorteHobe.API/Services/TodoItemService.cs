using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Portehobe.src.PorteHobe.API.DTOs;
using Portehobe.src.PorteHobe.Domain.Entities;
using Portehobe.src.PorteHobe.Infrastructure;

namespace Portehobe.src.PorteHobe.API.Services
{
    public class TodoItemService : ITodoItemService
    {
        private readonly AppDbContext _context;

        public TodoItemService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TodoItemDto>> GetUserTodosAsync(string userId)
        {
            return await _context.TodoItems
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt) // Newest first for a to-do list!
                .Select(t => new TodoItemDto
                {
                    Id = t.Id,
                    Description = t.Description,
                    IsDone = t.IsDone,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<TodoItemDto> CreateTodoAsync(CreateTodoItemDto dto, string userId)
        {
            var todo = new TodoItem
            {
                Description = dto.Description,
                UserId = userId
            };

            _context.TodoItems.Add(todo);
            await _context.SaveChangesAsync();

            return new TodoItemDto
            {
                Id = todo.Id,
                Description = todo.Description,
                IsDone = todo.IsDone,
                CreatedAt = todo.CreatedAt
            };
        }

        public async Task<bool> UpdateTodoAsync(int id, UpdateTodoItemDto dto, string userId)
        {
            var todo = await _context.TodoItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (todo == null) return false;

            todo.Description = dto.Description;
            todo.IsDone = dto.IsDone;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTodoAsync(int id, string userId)
        {
            var todo = await _context.TodoItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (todo == null) return false;

            _context.TodoItems.Remove(todo);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
