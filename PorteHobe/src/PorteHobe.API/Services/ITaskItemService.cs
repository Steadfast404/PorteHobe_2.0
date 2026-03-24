using System.Collections.Generic;
using System.Threading.Tasks;
using Portehobe.src.PorteHobe.API.DTOs;

namespace Portehobe.src.PorteHobe.API.Services
{
    public interface ITaskItemService
    {
        Task<IEnumerable<TaskItemDto>> GetUserTasksAsync(string userId);
        Task<TaskItemDto?> GetTaskByIdAsync(int id, string userId);
        Task<TaskItemDto> CreateTaskAsync(CreateTaskItemDto dto, string userId);
        Task<bool> UpdateTaskAsync(int id, UpdateTaskItemDto dto, string userId);
        Task<bool> DeleteTaskAsync(int id, string userId);
    }
}
