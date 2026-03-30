using Portehobe.src.PorteHobe.API.DTOs;

namespace Portehobe.src.PorteHobe.API.Services
{
    public interface ITodoItemService
    {
        Task<IEnumerable<TodoItemDto>> GetUserTodosAsync(string userId);
        Task<TodoItemDto> CreateTodoAsync(CreateTodoItemDto dto, string userId);
        Task<bool> UpdateTodoAsync(int id, UpdateTodoItemDto dto, string userId);
        Task<bool> DeleteTodoAsync(int id, string userId);
    }
}
